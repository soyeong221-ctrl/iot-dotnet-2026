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

namespace WpfCafeKiosk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // 무슨 메뉴를 클릭하든 전부 이 이벤트 발생
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button; // 이벤트를 발생시킨 주체를 할당

            string[] tag = btn.Tag.ToString().Split("|");

            string menuName = tag[0];
            int price = int.Parse(tag[1]);
            string imagePath = tag[2];

            // MessageBox.Show($"{price}", $"{name}");
            MenuOptionWindow win = new MenuOptionWindow(menuName, price, imagePath);

            win.Owner = this; // MainWindow가 MenuOptionWindow의 부모

            bool? result = win.ShowDialog();

            // TODO
        }

       
    }
}