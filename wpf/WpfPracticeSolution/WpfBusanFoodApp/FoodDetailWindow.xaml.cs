using System.Diagnostics;
using System.Windows;
using CefSharp;
using MahApps.Metro.Controls;
using WpfBusanFoodApp.Helpers;
using WpfBusanFoodApp.Models;

namespace WpfBusanFoodApp
{
    public partial class FoodDetailWindow : MetroWindow
    {
        private readonly FoodItem foodItem;

        public FoodDetailWindow(FoodItem selectedFood)
        {
            InitializeComponent();

            foodItem = selectedFood;

            // XAML Binding의 기준 데이터 설정
            DataContext = foodItem;

            // 상세 설명 HTML 태그 제거
            TxtItemContents.Text = Common.ConvertHtmlToText(foodItem.ItemCntnts);

            // 지도 표시
            LoadMap(foodItem);
        }

        // 맛집의 위도/경도를 이용해서 지도를 표시하는 메서드
        private void LoadMap(FoodItem food)
        {
            if (food.Lat == 0 || food.Lng == 0)
            {
                MapBrowser.LoadHtml("<html><body>지도 정보가 없습니다.</body></html>");
                return;
            }

            // Leaflet.js를 이용해 OpenStreetMap 지도 표시.
            // 인터넷 연결 필요.
            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <link rel='stylesheet'
          href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />

    <style>
        html, body, #map {{
            margin: 0;
            width: 100%;
            height: 100%;
        }}
    </style>
</head>
<body>
    <div id='map'></div>

    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>

    <script>
        var map = L.map('map').setView([{food.Lat}, {food.Lng}], 16);

        L.tileLayer('https://tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            attribution: ''
        }}).addTo(map);

        L.marker([{food.Lat}, {food.Lng}]).addTo(map);
    </script>
</body>
</html>";

            // 만든 HTML을 CefSharp 브라우저에 로드.
            MapBrowser.LoadHtml(html);
        }

        private void Homepage_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(foodItem.HomepageUrl))
            {
                // ... (알림 메시지)
                return;
            }

            try
            {
                string url = foodItem.HomepageUrl.Trim();

                // URL이 http/https로 시작하지 않는 경우 보정
                if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    url = "http://" + url;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // 중요: Windows에서 브라우저를 실행하려면 true 필수
                });
            }
            catch (Exception ex)
            {
                // 디버깅
                MessageBox.Show($"홈페이지를 열 수 없습니다: {ex.Message}", "오류");
            }
        }
    }
}