using MahApps.Metro.Controls.Dialogs;
using System.Configuration;
using System.Data;
using System.Windows;
using WpfMvvm02.ViewModels;
using WpfMvvm02.Views;

namespace WpfMvvm02 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            // App() 생성자에 MVVM을 추가, 생성자 이후 바로 실행되는 Startup이벤트에 작성해도됨
            MainView view = new MainView();
            view.DataContext = new MainViewModel(DialogCoordinator.Instance);
            view.ShowDialog();
        }
    }

}