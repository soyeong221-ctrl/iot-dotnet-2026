# 토이 프로젝트

## WPF MVVM 패턴 활용

### MVVM 패턴 개요
- MVC 패턴의 확장
    - C++, C#, Winforms 예전 MVC 따로 사용
    - 팀으로 개발할 때 디자인 작업. 개발 작업 분리 - 공백을 줄이고자
    - 장점: 유지보수 시 구분된 레이어만 수정하면 됨
    - 단점: 코드가 복잡해짐. 개발보다 구현이 쉽지 않음

- MVVM - Model - View - ViewModel 패턴
    - MVC 패턴과의 차이점 - Controller 대신인 ViewModel이 아니고 `View`가 대문
    - View에서 동작의 처리를 시작. **이벤트 핸들러가 모두 사라짐**
    - View에 해당하는 xaml.cs 파일에는 아무런 로직이 안 들어감.(디자이너는 로직을 생각하지 말 것)
    - 버튼, 키보드 등 이벤트가 모두 ViewModel로 넘어감 -> Command
    - 단점: 디버깅이 조금 어려움(몇몇 상태는 디버깅이 안 됨)
    
![alt text](image-290.png)

- MVVM 라이브러리 - 손쉽게 MVVM 구현을 도와주는 역할
    - `CommunityToolkit.Mvvm` - MS 개발. 가장 일반적. 난이도 하
    - Prism - MS관련 개발. 중대형 비즈니스용. 난이도 상
    - Caliburn.Micro - 간단한 MVVM 패키지. 난이도 하
    - Avalonia - 크로스플랫폼용 MVVM. 난의도 중


### MVVM 초간단 예제
- CommunityToolkit.Mvvm 패키지 설치
- Models, Views, ViewModels 폴더(네임스페이스) 생성

#### Model 작성

```cs
namespace WpfMvvm01.Models
{
    public class Person
    {
        public string Name { get; set; }
    }
}
```

#### ViewModel 작성
```cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfMvvm01.ViewModels
{
    // Ovservable(객체내용 변경 추적) 
    // MainViewModel이 다른 클래스와 함쳐져서 컴파일 됨
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string message = "안녕하세요.";
    }
}
```

#### View 생성
- Views/MainView.xanl 생성

#### App.xaml 수정
- StartupUri 속성 삭제
- App.xaml.cs 생성자 추가

```cs
public App()
{
    MainView view = new MainView();
    // MainView 객체의 전체데이터를 관장하는 DataContext에 ViewModel을 할당
    view.DataContext = new MainViewModel();
    view.Show();
}
```

#### MainView.xaml 수정

```xml
<TextBox FontSize="30" Text="{Binding Message}" />
```

- INotifyPropertyChanged 인터페이스 내 PropertyChanged 이벤트가 실행

#### 실행결과

![alt text](image-276.png)

#### ViewModel에 버튼클릭 로직 추가
- MVVM은 Click이벤트 사용 안 함. 대신 Command 사용

```cs
public partial class MainViewModel : ObservableObject
{
    ...

    [RelayCommand] // View에서 넘어온 명령을 처리
    private void ChangeMessage()
    {
        Message = "버튼 클릭!!!";
    }
}
```

#### View에 버튼 추가
- ViewModel의 RelayCommand 메서드명 + Command 단어입력 필수
- 비동기명령 메서드는 Async는 생략가능
    - ChangedMessage`Async` -> ChangeMessage`Command`

```xml
<Button Content="변경" Command="{Binding ChangeMessageCommand}">
```

#### 실행결과

![alt text](image-279.png)

- View는 디자이너 작업 - UI 설게서에 따라 속성값만 Binding으로 입력
- ViewModel은 개발자 작업 - 속성은 ObservableProperty로. 명령은 메서드(Command 제거)로 작성

#### 양방향 바인딩
- View에서 입력한 데이터를 ViewModel을 통해 Model로 전달하기 위해서 사용

```xml
<TextBox FontSize="30" Text="{Binding Message, UpdateSourceTrigger=PropertyChanged}" />
<TextBlock FontSize="30" Foreground="Blue" Text="{Binding Message}" />
```

![alt text](image-280.png)

#### ListView 데이터 바인딩
- ViewModel에 ObservableCollection 사용

```cs
public ObservableCollection<Person> People { get; } =
[
    new Person {Name = "홍길동"}, 
    new Person {Name = "가나디"}, 
    new Person {Name = "고먀미"}, 
    new Person {Name = "박소영"},
];
```

- View에 ListView 컨트롤 추가

```xml
<ListView ItemsSource="{Binding People}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Name}" FontSize="20"  />
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

- ViewModel에 선택항목 표시 속성

```cs
[ObservableProperty]
private Person? selectedPerson;
```

- View 선택항목 표시 컨트롤 추가

```xml
<TextBlock FontSize="20" Foreground="CadetBlue" Text="{Binding SelectedPerson.Name}" />
```

- 실행화면

![alt text](image-282.png)


### 책 대여 시스템 MVVM

#### 필요 패키지
- CommunityToolkit.Mvvm
- MahApps.Metro
- MahApps.Metro.IconPacks
- MySQLConnector
- Nlog

#### MahApp.Metro 디자인 지정
- App.xaml 추가

#### 패턴 폴더 생성
- Models, Views, ViewModels

#### MVVM 패턴에서 다이얼로그 처리
- MVVM 패턴에서 MahApps.Metro의
    - this.ShowMessageAsync() 메서드 사용 불가
- MVVM 패턴에 맞춰서 설정

- App.xaml.cs에서 MainViewModel 객체 생성시 파라미터 추가
```cs
view.DataContext = new MainViewModel(DialogCoordinator.Instance);
```

- MainViewModel에 IDialogCoordinator 인터페이스 할당받는 생성자 추가
```cs
private readonly IDialogCoordinator _coordinator;

public MainViewModel(IDialogCoordinator coordinator) {
    title = "BookRentalShop v1.1";
    this._coordinator = coordinator; // App.xaml.cs에서 생성하면서 넘어온 파라미터를 초기화
}

// 메서드 내 사용법
[RelayCommand]
public async Task AppExit() {
    //MessageBox.Show("프로그램을 종료합니다.");

    await this._coordinator.ShowMessageAsync(this, "종료확인", "종료하시겠습니까?");
}
```

- MainView.xaml mah:MetroWindow 태그에 다이얼로그 속성 추가
```xml
<mah:MetroWindow x:Class="WpfMvvm02.Views.MainView"
       ...
        Title="{Binding Title}" Height="550" Width="1000"
        mah:DialogParticipation.Register="{Binding}">
```

- 실행결과

![alt text](image-283.png)

#### 메인영역 화면 전환
- Page로 화면 전환은 Frame 컨트롤 사용
- UserControl로 화면전환은 ContentControl 컨트롤 사용(화면 변경)

- MainView 화면
```xml
 <!--메인 영역(장르관리, 도서관리, 회원관리, 대여관리)-->
 <ContentControl Grid.Row="1" Content="{Binding CurrentView}"></ContentControl>
```

- MainViewModel 클래스
```cs
        // 메인화면 영역
        [ObservableProperty]
        private UserControl currentView;
        // public CurrentView 속성 자동생성
```

- MainView 메뉴 명령추가
```xml
<MenuItem Header="책장르" Command="{Binding ShowDivisionCommand}">
```

- MainViewModel.ShowDivision 메서드
```cs
public void ShowDivision() {
    //MessageBox.Show("TEST");
    var view = new DivisionView();
    view.DataContext = new DivisionViewModel(DialogCoordinator.Instance);


    CurrentView = view;
}
```

- 실행화면

![alt text](image-284.png)

#### 화면 복제

- View xmal 파일 복사. 이름 변경/클래스명 변경
- ViewModel 클래스 복사. 이름 변경/클래스명 변경
- MainView에서 메뉴 명령 추가
- MainViewModel에서 명령에 바인딩 되는 메서드 추가

#### 데이터 수정 후 변경표시 안 되는 오류

![alt text](image-285.png)

- 콤보박스 데이터바인딩된 컨트롤 데이터 선택시 바인딩 모드 문제발생
- 콤보박스 SelectedValue 기본 바인딩 모드 TwoWay. 저장 후 반영

```xml
<ComboBox 
    Grid.Row="1" Margin="3" 
    mah:TextBoxHelper.Watermark="장르명"
    ItemsSource="{Binding Divisions}"
    SelectedValuePath="DivCode"
    DisplayMemberPath="DivName"
    SelectedValue="{Binding SelectedBook.DivCode, 
                            UpdateSourceTrigger=PropertyChanged}"/>
```

- UpdateSourceTrigger=PropertyChanged 옵션. 없애도 됨
- 텍스트박스 Text 기본 바인딩모드 TwoWay. 직접 반영

- ViewModel에서 ObservableCollection<> 객체 생성, DB 데이터 로드전에 초기화 로직 잘못 작성해서 생긴 문제

#### Insert, Delete 기능 추가
- DB 데이터 저장 - BookIdx가 0이면 INSERT
- 초기화 기능 - SelectedBook 초기화
- 입력검증 - 쓰레기 데이터 저장 방지

![alt text](image-286.png)

- 삭제 기능 - 버튼 커스터마이징

```cs
new MetroDialogSettings {
    AffirmativeButtonText = "삭제",  // OK 대신
    NegativeButtonText = "취소"  // Cancel 대신
});
```

- 삭제버튼 활성화 토글 - 삭제확인 메시지창보다 버튼 자체 비활성화
- MVVM 기능 bool CanCommand() 사용으로 삭제여부 활성화 토글 

![alt text](image-287.png)

```cs
```cs
[ObservableProperty]
[NotifyCanExecuteChangedFor(nameof(DeleteCommand))]  // 변경알림
private Book selectedBook;

[RelayCommand(CanExecute = nameof(CanDelete))]
public async Task DeleteAsync() {
    // ...

public bool CanDelete() {
    return SelectedBook is { BookIdx: > 0 };
}
```
```

![alt text](image-288.png)


#### 예외처리
- [X] 저장버튼만 누르면 프로그램 종료
- [X] 입력검증시 컨트롤마다 메시지창 뜨는 비효율성


![alt text](image-289.png)