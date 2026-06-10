using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfCafeKiosk.Models;

namespace WpfCafeKiosk
{
    /// <summary>
    /// OrderConfirmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OrderConfirmWindow : Window
    {
        public ObservableCollection<OrderItem> Orders { get; }  // 부모창에서 넘어온 컬렉션은 여기에서 수정하지 않음

        public OrderConfirmWindow()
        {
            InitializeComponent();
        }

        public OrderConfirmWindow(ObservableCollection<OrderItem> orders)
        {
            InitializeComponent();  // 없으면 윈도우 디자인 안 그려짐

            Orders = orders;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LstConfirmOrder.ItemsSource = Orders;
            TxtTotalCount.Text = $"{Orders.Sum(x => x.Count)}잔";
            TxtTotalAmount.Text = $"{Orders.Sum(x => x.TotalPrice):N0}원";
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();

        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
