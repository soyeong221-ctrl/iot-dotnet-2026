using MahApps.Metro.Controls;
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
using WpfProductAdmin.Models;
using WpfProductAdmin.Services;

namespace WpfProductAdmin
{
    /// <summary>
    /// ProductEditWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProductEditWindow : MetroWindow
    {
        ApiService service;
        private Product _product;

        public ProductEditWindow(Product product)
        {
            InitializeComponent();

            service = new ApiService();

            // 전달받은 Product 데이터를 입력창에 할당
            _product = product;

            TxtProductName.Text = _product.ProductName;
            TxtCategory.Text = _product.Category;
            NudPrice.Value = Convert.ToDouble( _product.Price);
            NudStock.Value = Convert.ToInt32(_product.Stock);
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Product product = new Product
            {
                ProductName = TxtProductName.Text.Trim(),
                Category = TxtCategory.Text.Trim(),
                Price = Convert.ToDecimal(NudPrice.Value),
                Stock = Convert.ToInt32(NudStock.Value)
            };

            bool result = await service.PostProductAsync(product);  // 서비스에 메서드 추가

            if (result)
            {
                MessageBox.Show("상품이 등록되었습니다.");
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("상품 등록이 실패했습니다.");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}