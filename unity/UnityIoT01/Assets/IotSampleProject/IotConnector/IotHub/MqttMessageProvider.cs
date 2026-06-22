using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using M2Mqtt;
using M2Mqtt.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace IndustryCSE.IoT
{
    /// <summary>
    /// Use await messageProvider.SetModeAsync(false) to initialize in MQTT mode
    /// Use await messageProvider.SetModeAsync(true) to switch to simulation mode at runtime
    /// Use await messageProvider.SetModeAsync(false) to switch back to MQTT mode
    /// IoT 메시지를 MQTT 브로커에서 받거나, 시뮬레이션 모드에서 더미데이터 처리하는 기능의 클래스
    /// </summary>
    public class MqttMessageProvider : BaseMessageProvider
    {
        [SerializeField] private string _mqttBrokerEndpoint = "<YOUR_MQTT_BROKER_ADDRESS>";
        [SerializeField] private int _mqttBrokerPort = 1883;
        [SerializeField] private string _mqttSubTopic = "<YOUR_MQTT_Subscription_Topic>";
        [SerializeField] private string _mqttUserName = "<YOUR_MQTT_User_Name>";
        [SerializeField] private string _mqttPassword = "<YOUR_MQTT_Password>";

        [SerializeField] private bool _secured = true;

        private MqttClient _mqttClient; // MQTTnet 봤던 MqttClient와 동일한 객체
        private string _clientId;       
        private bool _isConnected;

        /// <summary>
        /// 시뮬레이션 모드와 MQTT모드 전환 메서드
        /// </summary>
        /// <param name="useSimulatedEvents">참이면 시뮬레이션 모드, 거짓이면 MQTT 수신 모드</param>
        public override async Task SetModeAsync(bool useSimulatedEvents)
        {
            _simulateEvents = useSimulatedEvents;

            if (_simulateEvents)
            {
                Debug.Log("시뮬레이션 모드 초기화");

                // MQTT 연결 해제
                DisconnectClient();

                // 시뮬레이션 메시지 콜백 이벤트 등록
                _deviceSimulator.OnDeviceMessage += ReadSimulatedMessage;
            }
            else
            {
                Debug.Log("MQTT 모드 초기화");

                // 시뮬레이션 이벤트 비활성화
                _deviceSimulator.OnDeviceMessage -= ReadSimulatedMessage;

                // MQTT 연결 초기화
                await InitializeAsync();
            }
        }

        /// <summary>
        /// Initializes MQTT client or sets up simulated events based on the `UseSimulatedEvents` setting.
        /// </summary>
        public override async Task InitializeAsync()
        {
            if (_simulateEvents)
            {
                Debug.Log("시뮬레이션 모드 초기화");
                _deviceSimulator.OnDeviceMessage += ReadSimulatedMessage;
                return;
            }

            Debug.Log("MQTT 모드 초기화");
            await InitializeMqttClientAsync();
        }

        /// <summary>
        /// Initializes the MQTT client asynchronously.
        /// </summary>
        private async Task InitializeMqttClientAsync()
        {
            try
            {
                _clientId = Guid.NewGuid().ToString();
                if (!_secured)
                    _mqttClient = new MqttClient(_mqttBrokerEndpoint, _mqttBrokerPort, false, null, null, MqttSslProtocols.None);
                else 
                    _mqttClient = new MqttClient(_mqttBrokerEndpoint, _mqttBrokerPort, true, null, null, MqttSslProtocols.TLSv1_2);

                // Register event for receiving messages
                _mqttClient.MqttMsgPublishReceived += OnMqttMsgPublishReceived;

                Debug.Log("MQTT 브로커 연결 중");

                Task connectTask = Task.Run(() => _mqttClient.Connect(_clientId, _mqttUserName, _mqttPassword));
                await connectTask;

                if (_mqttClient.IsConnected)
                {
                    _isConnected = true;
                    SubscribeToTopic(_mqttSubTopic);
                    Debug.Log("MQTT 클라이언트 연결 성공");
                }
                else
                {
                    Debug.LogError("MQTT 브로커 연결 실패");
                }
            }
            catch (SocketException se)
            {
                Debug.LogError($"[SocketException] MQTT 브로커 연결 실패: {se.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Exception] MQTT 클라이언트 초기화 실패: {e.Message}");
            }
        }

        /// <summary>
        /// Subscribes to the specified MQTT topic.
        /// </summary>
        /// <param name="topic">The topic to subscribe to.</param>
        private void SubscribeToTopic(string topic)
        {
            if (!_isConnected || _mqttClient == null) return;

            try
            {
                _mqttClient.Subscribe(new[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                Debug.Log($"Subscribed to MQTT topic: {topic}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to subscribe to MQTT topic '{topic}': {e.Message}");
            }
        }

        /// <summary>
        /// Reads a simulated message (used in simulation mode).
        /// </summary>
        private void ReadSimulatedMessage(string message)
        {
            if (IsPaused) return;

            DeviceMessage deviceMessage = CreateDeviceMessage(message);

            if (deviceMessage != null)
            {
                MessageReceived(deviceMessage);
            }
        }

        /// <summary>
        /// Handles incoming MQTT messages.
        /// </summary>
        private void OnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {

            if (IsPaused) return;

            try
            {
                string message = Encoding.UTF8.GetString(e.Message);
                DeviceMessage deviceMessage = CreateDeviceMessage(message);

                if (deviceMessage != null)
                {
                    MessageReceived(deviceMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing MQTT message: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a DeviceMessage object from a raw MQTT message.
        /// </summary>
        private DeviceMessage CreateDeviceMessage(string message)
        {
            // Debug.Log("Device Message " + message);

            string deviceId = "";

            try {
                JObject body = JsonConvert.DeserializeObject<JObject>(message);
                if (body.TryGetValue("deviceId", out JToken device_id) && !string.IsNullOrEmpty(device_id?.ToString()))
                {
                    deviceId = device_id.ToString();
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("Malformed MQTT message: Missing 'deviceId'.");
            }

            return new DeviceMessage(message) { DeviceId = deviceId };
        }

        /// <summary>
        /// Cleans up resources when the application quits.
        /// </summary>
        private void OnApplicationQuit()
        {
            DisconnectClient();
        }

        /// <summary>
        /// Properly disconnects the MQTT client.
        /// </summary>
        private void DisconnectClient()
        {
            if (_mqttClient != null)
            {
                try
                {
                    if (_mqttClient.IsConnected)
                    {
                        _mqttClient.Disconnect();
                        Debug.Log("MQTT client disconnected.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error disconnecting MQTT client: {e.Message}");
                }
                finally
                {
                    _mqttClient = null;
                }
            }
        }
    }
}
