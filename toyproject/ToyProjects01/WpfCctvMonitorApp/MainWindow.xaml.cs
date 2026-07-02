using LibVLCSharp.Shared;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;
using WpfCctvMonitorApp.Common;
using WpfCctvMonitorApp.Models;
using WpfCctvMonitorApp.Services;

namespace WpfCctvMonitorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        #region "변수 영역"
        // readonly - 생성자에서만 초기화 가능, 이후 변경 불가
        private LibVLC libVLC;
        private LibVLCSharp.Shared.MediaPlayer mediaPlayer;

        private ItsCctvService itsCctvService;
        #endregion

        #region "기본 생성자 및 이벤트핸들러 영역"
        // 지역 선택한 위경도 범위 저장할 속성
        private GeoBound selectedGeoBound;  // 대문자=속성, 소문자=변수

        public MainWindow()
        {
            InitializeComponent();  // 절대 손대지 말 것!

            InitLivVlc();   // VLC라이브러리 초기화. 메서드를 분리하면 readonly 사용불가
            InitWebApiService();    // 웹서비스 초기화

        }

        /// <summary>
        /// 윈도우 로드완료 후 이벤트핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: 나중에 지울 것 - VideoView에 ITS페이지 스트리밍 띄우기
            //var media = new Media(libVLC, new Uri("https://cctvsec.ktict.co.kr:8082/mstrm0301hls/155C48ACE14C5708C9DF794025D2B2D6/live/04CT000001326/002/SELF/playlist.m3u8?nimblesessionid=23799895&wmsAuthSign=c2VydmVyX3RpbWU9Ny8xLzIwMjYgMzo1MzoxNiBBTSZoYXNoX3ZhbHVlPTlaYURtVW8yNUdmOFlRSUtubGJoQVE9PSZ2YWxpZG1pbnV0ZXM9MTIwJmlkPW1sdG0jbnRpY2xpdmUjNzI4Nzc="));
            //mediaPlayer.Play(media);
            await InitWebview2Async();

            var result = InitApiKey();
            if (!result)
            {
                return;
            }
            InitStatusBar();   // xmal 화면 텍스트들 초기화
            InitComboItems();   // 콤보박스 지역 리스트업 
        }

        private void BtnExpress_Click(object sender, RoutedEventArgs e)
        {
            Common.AppCommon.RoadType = "ex";
        }

        private void BtnNational_Click(object sender, RoutedEventArgs e)
        {
            Common.AppCommon.RoadType = "its";
        }

        private void CboRegions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Debug.WriteLine(CboRegions.SelectedItem);
            if (CboRegions.SelectedIndex > 0)   // -- 선택 -- 제외, 선택되었을 때 이벤트 발생
            {
                // MessageBox.Show(CboRegions.SelectedValue.ToString());
                selectedGeoBound = GetRegionBounds(CboRegions.SelectedValue.ToString());

                Debug.WriteLine(selectedGeoBound.MinLat);
                Debug.WriteLine(selectedGeoBound.MaxLat);
                Debug.WriteLine(selectedGeoBound.MinLng);
                Debug.WriteLine(selectedGeoBound.MaxLng);
            }
        }
        private void TglRoadType_Checked(object sender, RoutedEventArgs e)
        {
            AppCommon.RoadType = "ex";
            // GrbCctv.Header = $"고속도로 CCTV 목록  (총 0건)";
            TglRoadType.Content = "🛣 고속도로";
        }

        private void TglRoadType_Unchecked(object sender, RoutedEventArgs e)
        {
            AppCommon.RoadType = "its";
            // GrbCctv.Header = $"국도 CCTV 목록  (총 0건)";
            TglRoadType.Content = "🛡 국도";
        }

        /// <summary>
        /// 정상적으로 실행되면 처리하는 이벤트
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">컨트롤/객체 소유 속성들</param>
        private void mediaPlayer_Playing(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
                {

                    TxtConnStat.Text = "● 연결 상태 : 정상";
                    TxtConnStat.Foreground = new SolidColorBrush(Colors.Green);
                });
        }

        // 실행 안 되면 처리하는 이벤트
        private void mediaPlayer_EncounteredError(object? sender, EventArgs e)
        {
            // UI 스레드와 충돌(응답없음) 방지
            Dispatcher.Invoke(() =>
            {

                // MessageBox.Show("CCTV 스트리밍 재생 중 오류가 발생했습니다.");
                // 상태표시 빨간색으로 변경
                TxtConnStat.Text = "● 연결 상태 : 불량";
                TxtConnStat.Foreground = new SolidColorBrush(Colors.Red);
            });
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Validation Check
            if (selectedGeoBound == null)
            {
                System.Windows.MessageBox.Show("지역을 선택하세요.", "오류");
                return;
            }
            // 아니면 try-catch로 예외처리

            AppCommon.MinX = selectedGeoBound.MinLng;
            AppCommon.MaxX = selectedGeoBound.MaxLng;
            AppCommon.MinY = selectedGeoBound.MinLat;
            AppCommon.MaxY = selectedGeoBound.MaxLat;


            try
            {
                var totalApiUrl = AppCommon.BuildCctvApiUrl();

                var result = await itsCctvService.GetCctvListAsync(totalApiUrl);
                Debug.WriteLine(result);

                // MessageBox.Show(result.Response.DataCount.ToString());

                LsbCctv.ItemsSource = result.Response.Data;
                var roadName = AppCommon.RoadType == "ex" ? "고속도로" : "국도";
                GrbCctv.Header = $"{roadName} CCTV 목록  (총 {result.Response.DataCount:N0}건)";
            }
            catch (Exception ex)
            {
                // 대부분 OpenAPI 호출 이후 네트워크문제에서 발생
                System.Windows.MessageBox.Show($"CCTV 검색 중 오류가 발생하였습니다. {ex.Message}", "검색 오류",
                                System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 리스트박스 선택 후 이벤트
        private async void LsbCctv_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LsbCctv.SelectedItem is not CctvInfo selected)
            {
                return;
            }

            // MessageBox.Show(selected.CctvUrl);
            PlayCctv(selected.CctvUrl);

            await ShowMarkerAsync(selected);

            SetDetailInfo(selected);
            DisplayStatusBarInfo(selected);
        }

        /// <summary>
        /// 초기화 버튼 클릭 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInit_Click(object sender, RoutedEventArgs e)
        {
            TglRoadType.IsChecked = true;
            CboRegions.SelectedIndex = 0;
            GrbCctv.Header = $"CCTV 목록  (총 0건)";
            LsbCctv.ItemsSource = null; // ItemsSource로 바인딩했을 때는 Items.Clear() 사용불가
            mediaPlayer.Stop();
            WvwMap.Reload();    // WebBrowser F5

            SetDetailInfo();
            InitStatusBar();

        }

        // 종료여부 확인
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = System.Windows.MessageBox.Show("종료하시겠습니까?", "종료 확인", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.No)
            {
                e.Cancel = true; // 닫기 취소
                return;
            }
        }

        // 프로그램 종료시 메모리 해제
        private void Window_Closed(object sender, EventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Dispose();
            libVLC.Dispose();
        }
        #endregion

        #region "커스텀 메서드 영역"
        /// <summary>
        /// ITS OpenAPI 서비스 초기화 메서드
        /// </summary>
        private void InitWebApiService()
        {
            // OpenAPI 서비스 객체 생성
            itsCctvService = new ItsCctvService();
        }

        /// <summary>
        /// LibVLCsharp.WPF 컨트롤 초기화 메서드
        /// </summary>
        private void InitLivVlc()
        {
            // LibVLCsharp 초기화
            libVLC = new LibVLC();
            mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(libVLC);
            mediaPlayer.Playing += mediaPlayer_Playing;
            mediaPlayer.EncounteredError += mediaPlayer_EncounteredError;

            VvwScreen.MediaPlayer = mediaPlayer;
        }


        private static bool InitApiKey()
        {
            AppCommon.ItsApiKey = ConfigurationManager.AppSettings["ItsApiKey"];

            if (string.IsNullOrWhiteSpace(Common.AppCommon.ItsApiKey))
            {
                System.Windows.MessageBox.Show("ITS API Key가 설정되지 않았습니다.");
                return false;
            }
            // MessageBox.Show(Common.AppCommon.ItsApiKey);
            return true;
        }

        private async Task InitWebview2Async()
        {
            // WebView2 초기화
            await WvwMap.EnsureCoreWebView2Async();

            string html = GetLeafletHtml();
            WvwMap.NavigateToString(html);
        }

        private void InitStatusBar()
        {
            GrbCctv.Header = "CCTV 목록  (총 0건)";

            TxtConnStat.Text = "● 연결 상태 : 미연결";
            TxtConnStat.Foreground = new SolidColorBrush(Colors.Gray);

            TxtSelCctvName.Text = "선택 CCTV: 없음";
            TxtSelCctvUrl.Text = "영상 URL: 없음";

            TxtLastUpdateStat.Text = $"마지막 업데이트: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
        }

        // Leaflet 지도 HTML 문자열 생성 메서드
        private string GetLeafletHtml()
        {
            // """ or @" - 여러 줄 문자열 사용시 문자열내부에서 사용 가능
            return """
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <meta charset='utf-8' />
                                <style>
                                    html, body, #map {
                                        width: 100%;
                                        height: 100%;
                                        margin: 0;
                                        padding: 0;
                                    }
                                </style>

                                <link rel='stylesheet'
                                      href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />

                                <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
                            </head>
                            <body>
                                <div id='map'></div>

                                <script>
                                    let map = L.map('map').setView([36.5, 127.8], 7);

                                    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                                        maxZoom: 19
                                    }).addTo(map);

                                    let marker = null;
                                    
                                    // 새로 생성되는 마커 위치로 이동하는 함수
                                    function moveMarker(lat, lng, name) {
                                        if (marker != null) {
                                            map.removeLayer(marker);
                                        }

                                        marker = L.marker([lat, lng])
                                            .addTo(map)
                                            .bindPopup(name)
                                            .openPopup();

                                        console.log(lat, lng);

                                        map.setView([lat, lng], 14);
                                    }
                                </script>
                            </body>
                            </html>
                            """;
        }

        private void InitComboItems()
        {
            // CboRegions.Items.Add("전국");
            CboRegions.ItemsSource = Common.AppCommon.Regions;
            CboRegions.SelectedIndex = 0;
        }

        #endregion

        // 위경도 범위 리턴 메서드
        private GeoBound GetRegionBounds(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                return AppCommon.RegionBounds["전국"]; // 기본값으로 전국 범위 반환

            /*
            AppCommon.RegionBounds.TryGetValue(regionName, out GeoBound bound);

            if(bound == null)
            {
                return AppCommon.RegionBounds["전국"];
            }
            else { return bound; } 
            */

            return AppCommon.RegionBounds.TryGetValue(regionName, out GeoBound? bound)
                ? bound
                : AppCommon.RegionBounds["전국"];
        }

       

        private void DisplayStatusBarInfo(CctvInfo cctv)
        {
            TxtSelCctvName.Text = $"선택 CCTV: {cctv.CctvName}";
            TxtSelCctvUrl.Text = $"영상 URL: {AppCommon.Ellipsis(cctv.CctvUrl, 80)}";
        }

        private void SetDetailInfo(CctvInfo? cctv = null)
        {
            if (cctv != null)   // 실제 데이터 할당
            {
                TxtCctvName.Text = cctv.CctvName;
                TxtRoadName.Text = "";
                TxtDirection.Text = "";
                TxtCoordY.Text = cctv.CoordY;
                TxtCoordX.Text = cctv.CoordX;
                TxtCctvFormat.Text = cctv.CctvFormat;
            }
            else  // 초기화
            
            {
                TxtCctvName.Text = string.Empty;
                TxtRoadName.Text = "";
                TxtDirection.Text = "";
                TxtCoordY.Text = string.Empty;
                TxtCoordX.Text = string.Empty;
                TxtCctvFormat.Text = string.Empty;
            }
        }

        private async Task ShowMarkerAsync(CctvInfo cctv)
        {
            double lat = Convert.ToDouble(cctv.CoordY); // 위도
            double lng = Convert.ToDouble(cctv.CoordX); // 경도

            string name = cctv.CctvName ?? "CCTV";

            string script = $"moveMarker({lat}, {lng}, '{name}');";
            // string script = "alert('TEST')";
            await WvwMap.ExecuteScriptAsync(script);    // HTML 스크립트문을 외부에서 실행
        }

        // 스트리밍 재생 메서드
        private void PlayCctv(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            try
            {
                mediaPlayer.Stop(); // 이전 영상 멈추기
                using var media = new Media(libVLC, new Uri(url));
                var isPlayed = mediaPlayer.Play(media);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"오류 발생: {ex.Message}");
                return;
            }


        }

       
    }
}