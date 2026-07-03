# 웹 통합 토이프로젝트


## 국가교통정보센터 CCTV 정보앱


### 개요
- 국가교통정보센터에서 제공하는 OpenAPI를 통합해서 운영하는 RESTAPI서비스와 모니터링 앱 통합개발
- 국가교통정보센서 OpenAPI, 경찰청 도시교통정보센터 OpenAPI 통합해서 사용 가능

### 사용기술
- C# 14(.NET 10.0)
- WPF
- Wrapping RESTAPI 서비스
- ProgressBar?
- Newtonsoft.Json
- LibVLCSharp.WPF
- ITS 국가교통정보센터 OpenAPI - [링크](https://www.its.go.kr/)
- 경찰청 도시교통정보센터 OpenAPI - [링크](https://www.utic.go.kr/guide/newUtisDataWrite.do)
- MahApps.Metro? WPF UI?


### 개발환경 설정


#### 국가교통정보센터 사이트 회원가입


#### 로그인 후 인증키 신청
- 오픈데이터 > 오픈데이터 목록 > CCTV 화상자료 > 인증키 신청

![alt text](image-253.png)

![alt text](image-254.png)

##### 마이페이지 확인

![alt text](image-255.png)


#### Visual Studio

##### WPF 프로젝트 생성

![alt text](image-256.png)


#### 동영상 플레이 라이브러리
- 실시간 스트리밍(HLS), 동영상(mp4) 모두 재생 가능한 라이브러리 필요
- WPF MediaElement - HLS 재생 어려움. mp4 가능. 이미지 별도
- WebView2 - HLS 확인필요. mp4 가능. 이미지 가능
- FFME - HLS, mp4 가능. 이미지 별도
- `LibVLCSharp`.WPF - HLC, mp4 가능. 이미지 별도

##### VLC 
- [링크](https://www.videolan.org/)
- VideoLAN Organization에서 제작한 크로스 플랫폼 멀티미디어 재생 툴
- 스트리밍, 동영상 재생, 이미지 로드 가능

![alt text](image-257.png)

##### NuGet 패키지 설치
- Newtonsoft.Json
- LibVLCSharp.WPF
- VideoLAN.LivVLC.Windows
- WebView2


### 화면 UI


#### 와이어프레임

![alt text](image-258.png)


### 기본 구현


#### 메인화면 디자인

![alt text](image-259.png)


#### 앱 구조 설계
- Common - 공통 함수나 공통 변수 네임스페이스(폴더)
- Models - OpenAPI Json 데이터 구조 모델 클래스 네임스페이스
- Services - API 호출 및 데이터 처리 서비스 동작 클래스 네임스페이스


#### 앱 구조별 구현
- Common/AppCommon.cs - [소스](./toyproject/ToyProjects01/WpfCctvMonitorApp/Common/AppCommon.cs)
- Models/CctvInfo.cs - [소스](./toyproject/ToyProjects01/WpfCctvMonitorApp/Models/CctvInfo.cs)
- Services/ItsCctvService.cs - [소스](./toyproject/ToyProjects01/WpfCctvMonitorApp/Services/ItsCctvService.cs)

##### 화면에 VLC 라이브러리 추가
```xml
<!-- vlc 네임스페이스 추가 -->
<Window x:Class="WpfCctvMonitorApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:vlc="clr-namespace:LibVLCSharp.WPF;assembly=LibVLCSharp.WPF"
        ...>

...
<!-- CCTV 영상영역 border -->
<vlc:VideoView x:Name="VideoView" />
```

##### 기본 구현
- 로딩 후 스트리밍 테스트

##### 비즈니스 로직에 구현
- type은 `실시간`, 동영상, 정지영상 모두 같은 CCTV를 표현하는 방법만 다름

0. App.config 에서 API key 로드
1. 고속도로/국도 선택
2. 지역 검색 - 지역별 최소/최대 위도, 최소/최대 경도 확인
    - 지역 선택으로 간결화
3. 상세필터 - 시/도로 최소/최대 위도와 경도 확인. (노선, 방향은 삭제)
4. 검색 - OpenAPI URL로 위경도 범위별 CCTV 조회
5. CCTV 목록 - 리스트
6. 리스트아이템 클릭 - CCTV 영상 플레이
7. 지도 영역 - CCTV 위치 지도 위 표시
8. CCTV 정보 - json 결과 추출 표시


#### App.config

![alt text](image-260.png)

- xml로 구성된 파일
- [소스](./toyproject/ToyProjects01/WpfCctvMonitorApp/MainWindow.xaml.cs)

##### UI 변경
- CCTV 목록 페이징 삭제
- 지역 검색 삭제
- 시/도 선택 -> 지역 선택 변경
- 노선, 방향 선택 삭제

##### GeoBound 클래스 생성
- 지역 선택 시 최소, 최대 위도/경도를 할당해주는 클래스 - [소스](./toyproject/ToyProjects01/WpfCctvMonitorApp/Common/GeoBound.cs)
- 지역 선택 콤보박스에 로직 추가

##### 검색 버튼 작성
- BtnSearch 명명 및 로직 추가
- ItsCctvService.cs - [소스](./toyproject/ToyProjects01/WpfCctvMonitorApp/Services/ItsCctvService.cs)
- json 매핑 모델 클래스 - CctvResponse.cs - [소스](./toyproject/ToyProjects01/WpfCctvMonitorApp/Models/CctvResponse.cs)

##### 중간결과 화면
- 도로구분 선택, 지역 선택 후 검색. CCTV 리스트 개수 출력

![alt text](image-261.png)

##### CCTV 목록아이템 템플렛
- ListBox 일반 ListBoxItem을 ListBox.ItemTemplate으로 변경
- 데이터 바인딩 {Binding CctvName}

- 국도 선택, 부산 선택 후 검색 결과

![alt text](image-262.png)

##### 리스트뷰 클릭 스트리밍 재생
- 클릭이벤트 생성 메시지창 출력

![alt text](image-263.png)

- LibVLCsharp.WPF에 전달. 스트리밍 플레이

##### 지도표시
- CefSharp(Chrominum) 웹브라우저는 설치용량이 큼
- WebView2(Edge runtime)이 상대적으로 용량 적음
- 브라우저 기능 전체 사용이 아닌 지도표시만 하면 WebView2가 적합

```xml
<Window x:Class="WpfCctvMonitorApp.MainWindow"
        ...
        xmlns:vlc="clr-namespace:LibVLCSharp.WPF;assembly=LibVLCSharp.WPF"
        xmlns:wv2="clr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.Wpf"
    ...
    <wv2:WebView2 x:Name="WvwMap" />
    ...
```
- WebView2 초기화 로직

![alt text](image-264.png)

- 리스트뷰 아이템 클릭시 상세지도 마커표시

![alt text](image-265.png)

##### CCTV 상세정보
- CctvInfo 내용을 출력
- TextBlcok에 Text 속성에 할당

##### 상태표시줄 작업
- 연결상태 - 리스트박스아이템 선택 후 스트리밍이 상태따라 변경
    - 스트리밍 안 되면 예외처리
- 선택 CCTV - CCTVName 그대로 사용
- 영상 URL - 전체 표시 -> 생략 + 일부 표시
- 마지막 업데이트 - 제외

##### 종료시 메모리 해제
- 메모리 누수 발생 가능성 제거
- OnClosed() 이벤트 객체 해제로직 추가

##### 예외처리
- 아래 리스트 수정
    - [x] 지역선택없이 검색하면 프로그램 종료
    - [x] API Key 누락되었을 때
    - [x] VLC 재생 실패 감지

##### 실행시 화면 텍스트 초기화
- 아래 리스트 수정
    - [x] CCTV 목록 (총 125건)
    - [x] 연결 상태 : 정상
    - [x] 선택 CCTV : 경부선. ..
    - [x] 영상 URL : ...

##### 리팩토링(Refactoring)
- 프로그램 기능은 그대로 유지하면서 내부 구조를 더 좋게 개선
    - 중복코드 제거
    - 메서드 소스를 하위메서드 생성으로 간결화
    - 하드코딩 제거
    - 로직 효율화

- Visual Studio에서 `Ctrl + .`/ **Alt + Enter**로 리팩토링 쉽게 사용 가능

![alt text](image-266.png)

##### 초기화 버튼 기능
- 지역 선택 초기화
- 고속도로 기본 토글버튼
- CCTV 목록 제거, 건수 초기화
- 상태바 초기화
- 영상정지
- 지도 초기화


### UI 변경
- WPF UI
- Light/Dark theme


### Nuget Package 설치
- 도구 > Nuget Package 관리자 > 패키지 관리자 콘솔

```powershell
PM> Install-Package WPF-UI
```

- App.xaml 태그 코드 추가
- MainWindow.xaml 부모 클래스 FluentWindow로 변경
- Light 테마 적용

##### 변환 결과

##### 추가 수정
- [x] 제목표시줄 추가 - WPF Ui 특성
- [x] 메시지박스 변경

    ![alt text](image-267.png)

    ![alt text](image-268.png)

- [x] CCTV 정보 글자 잘라서 표기

    ![alt text](image-269.png)

- [] 리스트박스 목록 크기, 텍스트, 즐겨찾기 정리

##### 프로그레스바
- 검색 후 리스트박스 항목 다 나오기 전까지 표시
- WPF UI 적용 후 반영


##### 즐겨찾기 DB 추가

##### 즐겨찾기 읽어오기

---

### OpenAPI 래핑 웹서비스


#### 브릿지 웹서비스 구현


##### ASP.NET Core API 프로젝트

##### WPF 앱 필요 클래스 가져오기
- 네임스페이스 현재 이름으로 변경 필수
    - AppCommon.cs
    - CctvInfo.cs
    - CctvResponse.cs
    - ItsCctvService.cs

##### Program.cs에 서비스 등록
- ItsCctvService 등록

##### appsettings.json
- Its서비스키 추가

##### ApiController 추가
- ItsCctvController.cs 클래스 생성

##### 실행결과
- WPF 결과 -> json 구조 변경

![alt text](image-270.png)


#### 이전 WPF 연계작업

##### API 웹서비스 CctvResultDto 가져오기
- WPF로 복사
- 오류 나는 부분들 전체 수정


#### 전체 다이어그램

![alt text](image-271.png)


#### 사용기술

|구분|기술|
|---|---|
|윈앱 UI| WPF(.NET 10), WPF UI Framework|
|통신| HTTP(HTTPS 확장 가능) |
|데이터형식 | JSON, 직렬화, 역직렬화 |
|브릿지서버 | ASP.NET Core Web API |
|웹서버| Kestrel(크로스플랫폼 웹서버) |
|설정관리 | appsettings.json, App.config(XML) |
|서비스호출 | HttpClient |
|외부 API | ITS 국가교통정보센터 OpenAPI |
|API 방식 | REST API |
|웹아키텍처 | Model-Service-Controller Layer |
