using MahApps.Metro.Controls;
using NLog;
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
using WpfBusanFestivalApp.Helpers;
using WpfBusanFestivalApp.Models;
using WpfBusanFestivalApp.Services;

namespace WpfBusanFestivalApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        // NLog 기본 객체 생성방법
        // private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly FestivalApiService service;

        // 객체 생성은 클래스 생성자에서 일반적으로 구현
        public MainWindow()
        {
            InitializeComponent();

            service = new FestivalApiService();
            // logger에서 쓸 수 있는 메서드 Info(), Trace(), Debug(), Warn(), Error()
            Common.logger.Info("부산 페스티벌정보앱 시작");
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Common.logger.Info("부산 페스티벌정보앱 로드 시작");

            //string? key = Environment.GetEnvironmentVariable("BUSAN_FESTIVAL_API_KEY");
            //Console.WriteLine(key);

            // Api 서비스 생성
            //FestivalApiService service = new FestivalApiService();
            //var festivals = await service.GetFestivalsAsync();
            //DgrFestival.ItemsSource = festivals;

            await SearchFestivalAsync();

            Common.logger.Info("공공데이터 API 데이터 로드 완료");

        }

        // 검색
        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            //WebTestWindow win = new WebTestWindow();
            //win.Owner = this;
            //win.ShowDialog();

            await SearchFestivalAsync();
        }

        // 검색기능 처리
        private async Task SearchFestivalAsync()
        {
            try
            {
                BtnSearch.IsEnabled = false;    // 검색이 완료될 때까지 다시 못 누르게 비활성화.

                // NumPageNo.Value == null ? 1 : NumPageNo.Value
                int pageNo = Convert.ToInt32(NumPageNo.Value ?? 1);
                int numOfRows = Convert.ToInt32(NumOfRows.Value ?? 10);
                
                var festival = await service.GetFestivalsAsync(pageNo, numOfRows);

                DgrFestival.ItemsSource = festival;

                Common.logger.Info($"Page: {pageNo}, Records: {numOfRows} 로드 완료");

                SbiStatus.Text = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {pageNo} 페이지 {festival.Count}건 로드 완료";

            }
            catch (Exception ex)
            {

                Common.logger.Error($"데이터 로드 실패 SearchFestivalAsyn() : {ex.Message}");

                SbiStatus.Text = $"로드 실패!!";
            }
            finally
            {
                BtnSearch.IsEnabled = true;
            }
        }

        private void DgrFestival_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DgrFestival.SelectedItems.Count == 0)
            {
                return;
            }

            FestivalItem detailItem = DgrFestival.SelectedItem as FestivalItem;

            FestivalDetailWindow win = new FestivalDetailWindow(detailItem);
            win.Owner = this;

            win.ShowDialog();   // 부모창으로 데이터를 넘길 게 아니면 Result true/false
            
        }
    }
}