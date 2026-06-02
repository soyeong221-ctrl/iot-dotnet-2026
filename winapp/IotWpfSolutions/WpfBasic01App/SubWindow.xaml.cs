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

namespace WpfBasic01App
{
    /// <summary>
    /// SubWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SubWindow : Window
    {
        public SubWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("클릭했어요!", "클릭이벤트", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("클릭했어요2!", "클릭이벤트", MessageBoxButton.OK, MessageBoxImage.Warning);

        }
    }
}
