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
using WpfBusanFestivalApp.Services;

namespace WpfBusanFestivalApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //string? key = Environment.GetEnvironmentVariable("BUSAN_FESTIVAL_API_KEY");
            //Console.WriteLine(key);

            // Api 서비스 생성
            FestivalApiService service = new FestivalApiService();

            var festivals = await service.GetFestivalsAsync();

            DgrFestival.ItemsSource = festivals;
        }
    }
}