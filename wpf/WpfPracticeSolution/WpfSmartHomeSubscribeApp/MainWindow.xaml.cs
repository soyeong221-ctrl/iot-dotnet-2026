using MahApps.Metro.Controls;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Bogus;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using WpfSmartHomeSubscribeApp.Models;
using System.Printing;
using System.Text.Json;
using WpfSmartHomeSubscribeApp.Helpers;
using MQTTnet;
using MySqlConnector;

namespace WpfSmartHomeSubscribeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool IsConnected { get; set; } // 접속여부 확인

        private CancellationTokenSource? _cts; // 스레드 캔슬객체: 비동기 작업 중지시켜주는 객체


        #region MQTT/DB 속성/변수들

        private IMqttClient? MqttClient { get; set; }
        private string MqttHost { get; set; } = "127.0.0.1";    // TxtMqttBrokerIp 텍스트박스의 IP로 변경되어야 함
        private int mqttPort { get; set; } = 1883;
        private string MqttUser { get; set; } = "root";
        private string MqttPassword { get; set; } = "mqtt123456";
        private string MqttTopic { get; set; } = "home/sensor";
        private string DbHost { get; set; } = "127.0.0.4";
        private string DbUser { get; set; } = "root";
        private string DbPassword { get; set; } = "mt123456";
        private string DbName { get; set; } = "smarthome";

        private DatabaseHelper db;

        #endregion

        #region 생성자 영역
        public MainWindow()
        {
            InitializeComponent();  // UI 초기화

            // 커스텀 초기화
            IsConnected = false;    // 접속 안한 상태
        }

        #endregion

        #region 이벤트핸들러 영역
        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            // 입력검증(Validation Check)
            if (string.IsNullOrWhiteSpace(TxtMqttBrokerIp.Text))
            {
                await this.ShowMessageAsync("오류", "MQTT브로커주소를 입력하세요.");
                Common.logger.Warn("MQTT브로커주소 미입력!");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtMqttTopic.Text))
            {
                await this.ShowMessageAsync("오류", "MQTT토픽을 입력하세요.");
                Common.logger.Warn("MQTT토픽 미입력!");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtDatabaseIp.Text))
            {
                await this.ShowMessageAsync("오류", "Database IP를 입력하세요.");
                Common.logger.Warn("MQTT브로커주소 미입력!");
                return;
            }

            MqttHost = TxtMqttBrokerIp.Text.Trim(); // MqttHost 값 127.0.0.1 -> UI에서 입력한 HostIP로 변경
            MqttTopic = TxtMqttTopic.Text.Trim();
            DbHost = TxtDatabaseIp.Text.Trim();

            if (IsConnected == false)
            {
                // Mqtt브로커 접속 시도
                await ConnectMqttAsync();

                IsConnected = true;
                TxtStatus.Text = "DISCONNECT";

                AddLogs("SYSTEM", "MQTT Subscribe 접속시작.");
                Common.logger.Info("MQTT Subscribe 처리시작.");
                SbiStatus.Text = "MQTT 연결 시작.";
            }
            else
            {
                IsConnected = false;
                TxtStatus.Text = "CONNECT";

                if (MqttClient != null && MqttClient.IsConnected)
                {
                    await MqttClient.DisconnectAsync();

                    AddLogs("SYSTEM", "MQTT 브로커 접속종료.");
                    Common.logger.Info("MQTT Subscribe 접속종료.");
                    SbiStatus.Text = "MQTT 연결 종료.";

                }
            }
        }

        #endregion

        #region 커스텀메서드 영역


        private void AddLogs(string topic, string payload)
        {

            Dispatcher.Invoke(() =>
            {
                // UI스레드와 충돌없이 텍스트 출력 방법
                Paragraph p = new Paragraph();

                p.Margin = new Thickness(0, 0, 0, 10); // bottom에 10여백

                p.Inlines.Add(  // 출력시간표시
                    new Run($"[{DateTime.Now:HH:mm:ss}] ")
                    {
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Colors.Blue)
                    });

                p.Inlines.Add(  // 토픽
                    new Run($"TOPIC: {topic}")
                    {
                        FontWeight = FontWeights.Bold,
                    });

                p.Inlines.Add(   // json 페이로드
                    new Run(payload)
                    {
                        FontFamily = new FontFamily("Consolas")
                    });

                RtbLog.Document.Blocks.Add(p);

                if (RtbLog.Document.Blocks.Count > 50)
                {
                    RtbLog.Document.Blocks.Remove(
                        RtbLog.Document.Blocks.FirstBlock);
                }

                RtbLog.ScrollToEnd();   // 리치텍스트박스 가장 마지막으로 포커스

            });
        }

        /// <summary>
        /// MQTT 브로커 접속 메서드
        /// </summary>
        private async Task ConnectMqttAsync()
        {
            var factory = new MqttClientFactory();
            MqttClient = factory.CreateMqttClient();

            // Subscribe 핵심 - 데이터가 Publish(배포)되면 곧바로 Subscribe(구독)
            // Subscribe 실행된 후 Payload가 넘어왔을 때 이벤트 처리
            MqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                string topic = e.ApplicationMessage.Topic;

                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                AddLogs(topic, payload);

                await SaveSensorDataAsync(payload);
            };

            var options = new MqttClientOptionsBuilder()
                .WithClientId($"WPF-Subscriber-{Guid.NewGuid()}")
                .WithTcpServer(MqttHost, mqttPort)
                .WithCredentials(MqttUser, MqttPassword)
                .WithCleanSession()
                .Build();

            await MqttClient.ConnectAsync(options);

            // Subscribe 옵션
            var subscribeOptions = factory
                .CreateSubscribeOptionsBuilder()
                .WithTopicFilter(MqttTopic)
                .Build();

            // Subscribe 실행
            await MqttClient.SubscribeAsync(subscribeOptions);

            // DB 설정
            db = new DatabaseHelper();
            db.connStr = $"Server={DbHost};" +   // 운영아이피로 바꾸세요
                                 "Port=3306;" +   // 운영포트로 변경할 것
                                 $"Database={DbName};" +
                                 $"User ID={DbUser};" +  // 운영DB 사용자로 변경
                                 $"Password={DbPassword};" +  // 패스워드 변경할 것
                                  "Charset=utf8mb4;";

            AddLogs("SYSTEM", "MQTT 구독 시작.");
        }

        /// <summary>
        /// Subscribe 데이터 DB 저장
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task SaveSensorDataAsync(string payload)
        {
            try
            {
                // JSON 역직렬화
                List<SensorData> sensors = JsonSerializer.Deserialize<List<SensorData>>(payload);

                if (sensors == null || sensors.Count == 0)
                {
                    AddLogs("ERROR", "수신된 데이터가 없습니다.");
                    return;
                }

                await using var conn = new MySqlConnection(db.connStr);
                await conn.OpenAsync();

                foreach (var sensor in sensors)
                {
                    string query = $@"INSERT INTO smarthome.sensor_data
                                       (home_id, room_name, sensing_datetime, temp, humid, created_at)
                                       VALUES(
                                        '{sensor.HomeId}', 
                                        '{sensor.RoomName}', 
                                        '{sensor.SensingDateTime}', 
                                        {sensor.Temp}, {sensor.Humid}, 
                                        CURRENT_TIMESTAMP);";

                    await using var cmd = new MySqlCommand(query, conn);

                    await cmd.ExecuteNonQueryAsync();
                }

                AddLogs("DB", $"{sensors.Count}건 DB저장 완료!");
            }
            catch (Exception ex)
            {
                AddLogs("ERROR", ex.Message);
            }
        }

        #endregion
    }
}