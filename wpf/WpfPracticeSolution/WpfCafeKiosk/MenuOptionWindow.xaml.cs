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
using WpfCafeKiosk.Models;  // namespace 다르면 using문으로 import 해야함

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
        private int menuId; // 메뉴 아이디 추가
        private int qty = 1;    // Quantity
        private string[] tags;

        public OrderItem SelectedOrder { get; set; }

        // 변수가 추가될 때마다 파라미터 계속 늘리는 문제
        [Obsolete("앞으로 사용 안 함")]
        public MenuOptionWindow(string menuName, int price, string imagePath, int menuId)
        {
            InitializeComponent();

            this.menuName = menuName;
            this.price = price;
            this.imagePath = imagePath;

            TxtMenuName.Text = menuName;
            TxtPrice.Text = $"{price:N0}원";
            TxtQty.Text = qty.ToString();

            ImgMenu.Source = new BitmapImage(new Uri(imagePath, uriKind: UriKind.RelativeOrAbsolute));
        }

        public MenuOptionWindow(string? strTag)
        {
            InitializeComponent();  // WPF 창에서 생성자 필수

            tags = strTag.Split('|');

            this.menuName = tags[0];
            this.price = Convert.ToInt32(tags[1]);
            this.imagePath = tags[2];
            this.menuId = Convert.ToInt32(tags[3]);

            TxtMenuName.Text = menuName;
            TxtPrice.Text = $"{price:N0}원";
            TxtQty.Text = qty.ToString();

            ImgMenu.Source = new BitmapImage(new Uri(imagePath, uriKind: UriKind.RelativeOrAbsolute));
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            if (qty <= 1) return;   // 수량은 1이하로 내려갈 필요 없음

            qty--;
            TxtQty.Text = qty.ToString();
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            qty++;  
            TxtQty.Text = qty.ToString();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            SelectedOrder = new OrderItem { 
                MenuId = menuId,
                MenuName = menuName,
                Price = price,
                Count = qty,
                TotalPrice = price * qty
            };

            DialogResult = true;
            Close();
        }
    }
}
