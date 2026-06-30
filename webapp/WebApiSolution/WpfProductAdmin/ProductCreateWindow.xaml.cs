using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using WpfProductAdmin.Models;
using WpfProductAdmin.Services;

namespace WpfProductAdmin
{
    /// <summary>
    /// ProductCreateWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProductCreateWindow : MetroWindow
    {
        ApiService service;

        public ProductCreateWindow()
        {
            InitializeComponent();

            service = new ApiService();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation Check
            if (string.IsNullOrEmpty(TxtProductName.Text.Trim()))
            {
                await this.ShowMessageAsync("입력오류", "상품명을 입력하세요.");
                // TxtProductName.Focus(); // 상품명 입력창에 포커스
                return;
            }

            if (string.IsNullOrEmpty(TxtCategory.Text.Trim()))
            {
                await this.ShowMessageAsync("입력오류", "카테고리를 입력하세요.");
                return;
            }

            if (!Decimal.TryParse(NudPrice.Value.ToString(), out decimal price))
            {
                await this.ShowMessageAsync("입력오류", "가격은 숫자로 입력하세요.");
                return;
            }

            if (Convert.ToDecimal(NudPrice.Value) <= 0)
            {
                await this.ShowMessageAsync("입력오류", "가격은 1000원 이상 입력하세요.");
                return;
            }

            if (!int.TryParse(NudStock.Value.ToString(), out int stock))
            {
                await this.ShowMessageAsync("입력오류", "재고는 숫자로 입력하세요.");
                return;
            }


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
                await this.ShowMessageAsync("저장", "상품이 등록되었습니다.");
                DialogResult = true;
                Close();
            }
            else
            {
                await this.ShowMessageAsync("저장", "상품 등록이 실패했습니다.");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}