using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using WpfProductAdmin.Models;

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