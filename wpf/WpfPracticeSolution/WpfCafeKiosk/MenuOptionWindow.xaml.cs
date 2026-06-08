using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfCafeKiosk
{
    /// <summary>
    /// MenuOptionWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MenuOptionWindow : Window
    {
        private string menuName;
        private int price;
        private string imagePath;
        private int qty = 1;    // Quantity


        public MenuOptionWindow(string menuName, int price, string imagePath)
        {
            InitializeComponent();

            this.menuName = menuName;
            this.price = price;
            this.imagePath = imagePath;

            TxtMenuName.Text = menuName;
            TxtPrice.Text = $"{price:N0}원";

            ImgMenu.Source = new BitmapImage(new Uri(imagePath, uriKind: UriKind.RelativeOrAbsolute));
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
