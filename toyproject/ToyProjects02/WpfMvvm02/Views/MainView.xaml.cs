using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace WpfMvvm02.Views {
    public partial class MainView : MetroWindow {
        public MainView() {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            // 일반적인 클릭이벤트와 View에서만 사용가능.
            this.ShowMessageAsync("ㅆㄸㄴㅆ", "TEST!!!");
        }
    }
}