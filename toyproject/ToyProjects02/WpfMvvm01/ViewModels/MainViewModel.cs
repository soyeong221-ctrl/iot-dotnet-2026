using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WpfMvvm01.Models;

namespace WpfMvvm01.ViewModels
{
    // Ovservable(객체내용 변경 추적) 
    // MainViewModel이 다른 클래스와 함쳐져서 컴파일 됨
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string message = "Hi Hi ~ ♡";  // Message 속성을 자동 생성

        [ObservableProperty]
        private Person? selectedPerson; 

        public ObservableCollection<Person> People { get; } =
        [
            new Person {Name = "토순이"}, 
            new Person {Name = "가나디"}, 
            new Person {Name = "고먀미"}, 
            new Person {Name = "박소영"},
        ];


        [RelayCommand]  // View에서 넘어온 명령 처리
        private void ChangeMessage()
        {
            Message = "버튼 클릭!";
        }
    }
}
