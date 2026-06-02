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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfBasic02Navi
{
    /// <summary>
    /// MenuPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void BtnMenu01_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Sub01Page.xaml", UriKind.RelativeOrAbsolute));
        }

        private void BtnMenu02_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Sub02Page.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}