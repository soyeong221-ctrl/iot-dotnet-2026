using Accessibility;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using WpfProductAdmin.Models;
using WpfProductAdmin.Services;

namespace WpfProductAdmin
{
    /// <summary>
    /// ProductWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProductWindow : MetroWindow
    {
        private ApiService service;
        private Product _product;

        public ProductWindow()
        {
            InitializeComponent();

            service = new ApiService();

            BtnDelete.Width = 0;
            BtnDelete.Margin = new Thickness(0);
            BtnDelete.Visibility = Visibility.Hidden;  // 신규등록 때 삭제버튼 숨기기           
        }

        public ProductWindow(Product product)
        {
            InitializeComponent();

            service = new ApiService();

            // 전달받은 Product데이터를 입력창에 할당
            _product = product;

            TxtProductName.Text = _product.ProductName;
            TxtCategory.Text = _product.Category;
            NudPrice.Value = Convert.ToDouble(_product.Price);
            NudStock.Value = Convert.ToInt32(_product.Stock);
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

            if (_product is null)  // 신규 생성. ProductCreateWindow BtnSave 기능
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
                    await this.ShowMessageAsync("저장", "상품이 등록되었습니다.");
                    DialogResult = true;
                    Close();
                }
                else
                {
                    await this.ShowMessageAsync("저장", "상품 등록이 실패했습니다.");
                }
            }
            else
            { // 수정. ProductEditWindow BtnSave 기능
                // 이전 원본 객체를 수정
                _product.ProductName = TxtProductName.Text.Trim();
                _product.Category = TxtCategory.Text.Trim();
                _product.Price = price;
                _product.Stock = stock;

                bool result = await service.UpdateProductAsync(_product);

                if (result)
                {
                    await this.ShowMessageAsync("저장", "상품이 수정되었습니다.");
                    DialogResult = true;
                    Close();
                }
                else
                {
                    await this.ShowMessageAsync("저장", "상품 수정에 실패했습니다.");
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var confirm = await this.ShowMessageAsync("삭제", $"[{_product.ProductName}] 상품을 삭제하시겠습니까?",
                                                        MessageDialogStyle.AffirmativeAndNegative);

            //MessageBox.Show(confirm.ToString());
            if (!(confirm == MessageDialogResult.Affirmative))
            {
                DialogResult = false;
                Close();
            }

            // 삭제
            bool result = await service.DeleteProductAsync(_product.ProductId);

            if (result)
            {
                await this.ShowMessageAsync("삭제", "상품이 삭제되었습니다.");
                DialogResult = true;
                Close();
            }
            else
            {
                await this.ShowMessageAsync("삭제", "상품 삭제에 실패했습니다.");
                DialogResult = false;
                Close();
            }
        }
    }
}