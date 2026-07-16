using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;
using WpfMvvm02.Views;

namespace WpfMvvm02.ViewModels {
    public partial class MainViewModel : ObservableObject {
        [ObservableProperty]
        private string title;

        // 메인화면 영역
        [ObservableProperty]
        private UserControl currentView;
        // public CurrentView 속성 자동생성

        [ObservableProperty]
        private string currentViewName;

        private readonly IDialogCoordinator _coordinator;

        public MainViewModel() {
            title = "BookRentalShop v1.0";
        }

        public MainViewModel(IDialogCoordinator coordinator) {
            title = "BookRentalShop v1.1";
            _coordinator = coordinator; // App.xaml.cs에서 생성하면서 넘어온 파라미터를 초기화
        }

        #region 'Command(명령) 처리 메서드'

        [RelayCommand]
        public async Task AppExit() {
            //MessageBox.Show("프로그램을 종료합니다.");
            var result = await _coordinator.ShowMessageAsync(this, "종료확인", "종료하시겠습니까?",
                                    MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative) {
                Application.Current.Shutdown();
            } else {
                return;
            }
        }

        [RelayCommand]
        public void ShowDivision() {
            // MessageBox.Show("TEST");
            var view = new DivisionView();
            view.DataContext = new DivisionViewModel(DialogCoordinator.Instance);


            CurrentView = view;
            CurrentViewName = "현재화면: 책장르 관리";
        }

        [RelayCommand]
        public void ShowBook() {
            var view = new BookView();
            view.DataContext = new BookViewModel(DialogCoordinator.Instance);

            CurrentView = view;
            CurrentViewName = "현재화면: 도서 관리";
        }

        #endregion
    }
}