using System.Configuration;
using System.Data;
using System.Windows;
using WpfMvvm01.ViewModels;
using WpfMvvm01.Views;

namespace WpfMvvm01
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MainView view = new MainView();
            // MainView 객체의 전체데이터를 관장하는 DataContext에 ViewModel을 할당
            view.DataContext = new MainViewModel();
            view.Show();
        }
    }
}
