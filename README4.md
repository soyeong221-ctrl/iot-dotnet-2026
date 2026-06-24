# 2026 닷넷 개발자 비즈니스앱 개발

## 1. 웹 개발 개요

### Web
- World Wide Web을 줄여서 부르는 단어
- 인터넷의 목적: 핵전쟁이 나더라도 데이터를 완전 소실하지 않고 보관하기 위해서 시작(ArpaNet)
- 인터넷에서 통신 및 데이터를 전달의 어려움
- 1990년 팀 버너스리가 WWW를 발표. 1980년대부터 연구해온 결과
- XML이 너무 복잡해서 사용이 불편 -> 개량화해서 HTML을 개발
- 2014년 이후 HTML5로 적용 중

### 웹 구조
- `프론트엔드` - 웹 기술등을 사용해서 사용자가 브라우저에서 볼 수 있는 눈에 보이는 화면을 개발 영역
- `백엔드` - 프론트엔드에 전달할 데이터나 동적인 화면을 생성 처리등 눈에 보이지 않는 개발 영역

### 프론트엔드 웹 기술
- `HTML` - HyperText Markup Language 약자. 링크로 페이지를 이동하는 기술 [구조]
- `CSS` - Cascaded Style Sheet 약자. HTML에 디자인을 적용시키는 기술 [디자인]
- `JavaScript` - 원래 HTML(클라이언트, 프론트엔드)에 동작을 지원해주는 기술로 나온 언어. JS로 호칭 [동작]
    - JS기술이 진보해서 현재는 서버개발, 앱개발 등 여러 방면으로 개발하는 언어로 발전

### 백엔드 웹 기술
- 웹 서버(서비스) 단을 개발하는 기술. 프로그래밍 언어별로 구분
- 보통 Server Page라는 용어 사용. JSP(Java Server Page), ASP(Active Server Pagge)
- Java - Java Bean > EJB > JSP > Spring > **Spring Boot**
-.NET - ASP(.NET 이전, VBScript) > ASP.NET(윈도우만) > `ASP.NET Core` (멀티플랫폼)
- Python - Flask(간단한 웹개발), django(기업 솔루션), **FastAPI**(OpenAPI 개발용)

### 웹서버, 웹 서비스
- 웹 서버 - 프론트엔드 + 백엔드로 사용자가 웹화면을 사용할 수 있도록 서비스
- `웹 서비스` - 백엔드로 데이터만 전달하는 서비스

### 웹을 사용하는 이유
- `설치가 필요 없음` - PC 프로그램은 설치파일. 모바일 앱은 앱스토어 설치절차 필요
    - 웹은 웹브라우저만 있으면 URL로 사용 가능
- `운영체제 독립적` - 운영을 하면 OS에 관계없이 사용 가능
    - WPF는 윈도우 종속적
- `업데이트가 쉬움` - 서버만 내용을 업데이트하면 사용자는 기존과 동일하게 사용만하면
    - 유지보수 비용이 낮음
- `데이터 공유` - 서버에 존재하는 데이터를 실시간으로 공유가능
    - 데이터포털 OpenAPI, 카카오톡, 네이버 지도, 구글 드라이브 .. 
- AI와 연결 쉬움 - 대부분의 AI 서비스가 웹API 형태로 제공


## 2. 웹 표준 기술


### HTML 기본


#### Live Server 설치
- VS Code에서 로컬 HTML 파일을 서버형식으로 보여주는 플러그인
- 확장 > `Live Server` 검색 후 설치
- html > 컨텍스트 메뉴 > Open with Live Server 클릭 

![alt text](image-174.png)


#### HTML 기본구조
- index.html - 대부분 웹페이지의 첫 페이지 파일
- VS Code, html 파일 생성 후!, tab키로 HTML 기본 코드 생성

```html
<!DOCTYPE html><!-- 문서가 HTML5 양식 선언 -->
<html lang="ko"><!-- 가장 root 태그 -->
<head><!-- 웹 페이지 설정구역 -->
    <meta charset="UTF-8"><!-- 페이지설정, 유니코드 사용 -->
    <!-- Responsive Web 사용. 화면크기에 따라 디자인이 알맞게 변형되는 웹 -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Document</title>
</head>
<body><!-- 웹 페이지 표현구역 -->
    
</body>
</html><!-- XML과 동일, 모든 태그는 <tag></tag> 로 구성 <tag /> -->
```

- head - 웹 페이지 설정할 태그 작성
- body - 웹 페이지 표현할 태그 작성


#### HTML 기본 태그
- 마크다운 문법 -> HTML 태그로 변경
- HTML에서는 space, enter 제대로 적용되지 않음

|태그 | 설명 |
|---|---|
|`<html>` | HTML 문서 시작 |
|`<head>` | 문서정보(설정) |
|`<title>` | 브라우저 제목 |
|`<meta>` | 문서 구성정보 |
|`<body>` | 화면에 표시될 내용 |
|`<h1>`~`<h6>` | 제목. 마크다운 #과 동일 |
|`<p>` | 문단 표현 |
|`<a>` | anchor의 prefix. 하이퍼링크. 다른 페이지로 이동 |
|`<img>` | 이미지 |
|`<video>`| 동영상 위치 태그 |
|`<div>` | 영역 구분. HTML5에서 가장 많이 쓰는 태그 |
|`<span>` | 인라인 영역 구분 |
|`<ul>` | 순서 없는 목록 시작. 마크다운 -와 동일 |
|`<ol>` | 정렬된 목록 시작. 마크다운 1. 와 동일 |
|`<li>` | 목록 항목 |
|`<br>` | 줄바꿈 |
|`<hr>` | 구분선(가로) |
|`<table>` | 표(테이블) 시작 태그 |
|`<tr>` | row. 한줄 태그 |
|`<th>` | header. 표 제목. 테이블의 칼럼 |
|`<td>` | cell. 표 내용. 테이블의 데이터 |
|`<iframe>`| html 내 다른 html 소스를 추가하는 기능 | 

- 공백 - `&nbsp;`로 공백 표현 키워드 사용

- lorem - 화면에 텍스트 꾸미는 작업 도와주는 스니펫
    - lorem20 - 임의 표준텍스트 20 단어 생성

![alt text](image-175.png)

#### HTML 입력 태그
- HTML에서 사용자 입력을 받기 위한 태그

|태그| 설명|
|---|---|
|`<form>`| 입력영역 태그 |
|`<label>`| 라벨 태그 |
|`<button>` | 버튼 태그 |
|`<textarea>` | 멀티라인 텍스트박스 |
|`<select>` | 콤보박스 시작태그 |
|`<option>` | 콤보박스 목록태그 |
|`<input>` | 가장 중요한 입력 태그. type 속성으로 여러 컨트롤로 분기 |

- input 타임 목록

|속성값|예제|설명|
|---|---|---|
|text | `<input type="text">` | 한 줄 텍스트 입력 |
|password | `<input type="password">` | 비밀번호 입력 |
|email | `<input type="email">` | 이메일주소 입력 |
|number | `<input type="number">` | 숫자 입력 |
|checkbox | `<input type="check">` | 체크박스 |
|radio | `<input type="radio">` | 라디오버튼 |
|file | `<input type="file">` | 파일업로드  |
|date | `<input type="date">` | 날짜선택 |
|hidden | `<input type="hidden">` | 페이지 내 숨김값 |
|submit | `<input type="submit">` | 등록/저장 버튼 |
|button | `<input type="button">` | 일반 버튼 |
|reset | `<input type="reset">` | 입력값 초기화 버튼 |

- button 태그와 input 중 type, submit, button, reset은 button 태그와 동일
- 웹에서 회원가입, 로그인, 게시판 등록 화면 등의 90%는 위 태그로만 구성.

![alt text](image-176.png)

- html에서 id에 같은 값을 써도 컴파일오류 발생 안 함. 이런 오류는 개발자 몫


### CSS


#### inputs.html 디자인 바꾸기
- Bootstrap - 트위터에서 개발한 UI library

- 아래 태그를 `<title>` 태그 아래 붙여넣기
```html
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-sRIl4kxILFvY47J16cr9ZwB07vP4J8+LH7qKQnuqkuIAvNWLzeN8tE5YBujZqJLB" crossorigin="anonymous">
```

- 아래 태그를 `</body>` 태그 위에 붙여넣기
```html
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/js/bootstrap.bundle.min.js" integrity="sha384-FKyoEForCGlyvwx9Hj09JcYn3nv7wiPVlz7YYwJrWVcXK/BmnVDxM+D2scQbITxI" crossorigin="anonymous"></script>
```

- 각 태그별 class를 적절하게 입력
```html
<!-- class 속성이 CSS를 적용시키는 속성 -->
<input type="text" name="userId" id="userId" class="form-control>

```

![alt text](image-177.png)

- CSS는 HTML에 디자인을 미려하게 변경하는 기술

- [소스](./webapp/WebTech_basic/css_exam.html)

![alt text](image-178.png)


### Javascript
- 웹페이지(HTML)을 동적으로 변경시켜주는 프로그래밍 언어
- 기본 문법은 C, C++과 동일. 변수 지정등은 Python과 동일
- Typescript, React, Node.js .. 활용되는 곳이 매우 많음


#### HTML 연결
- html에 `<script>` 태그를 사용해서 내부에 같이 표현하거나, 외부 js 파일을 연결
- 필요한 경우 웹브라우저(Chrome)에서 개발자도구(F12)로 확인할 것

![alt text](image-179.png)


#### 기본 문법
- 변수부터 객체까지 - [소스](./webapp/WebTech_basic/js_exam.html)


#### DOM
- Document Object Model의 약자. HTML 트리 구조를 객체로 만든 모델
- JS로 접근 가능 - [소스](./webapp/WebTech_basic/js_dom.html)

![alt text](image-180.png)


#### JS 목적
- 웹페이지 동적으로 만들기. HTML 요소 접근 내용 변경 등
- 사용자와 상호작용. 클릭, 드래그 등 이벤트 처리
- DOM 제어
- 서버와 데이터 통신
- 데이터 처리 및 검증. 입력값 검사, 데이터 가공, 계산 등


## 3. ASP.NET Core

### 개요

### 사용법

## 4. 웹 실습 프로젝트

### IoT 스마트홈 통합 플랫폼
- MQTT WPF + Unity + WebAPI 연동

### 공공데이터 통합 플랫폼
-  OpenAPI 서비스 + WPF 연동

### 스마트팩토리 MES 미니 플랫폼

### AI 비전 검사 시스템

### 실시간 채팅 시스템 + 챗봇 기능
