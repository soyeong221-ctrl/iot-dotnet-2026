using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfMvvm02.Models {
    public partial class Book : ObservableObject {
        [ObservableProperty]
        private int bookIdx;  // BookIdx 자동생성

        [ObservableProperty]
        private string author = string.Empty;

        [ObservableProperty]
        private string divCode = string.Empty;

        [ObservableProperty]
        private string divName = string.Empty;  // JOIN할 때 사용

        [ObservableProperty]
        private string bookName = string.Empty;

        [ObservableProperty]
        private DateTime releaseDt = DateTime.Today;

        [ObservableProperty]
        private string isbn = string.Empty;

        [ObservableProperty]
        private decimal price;
    }
}