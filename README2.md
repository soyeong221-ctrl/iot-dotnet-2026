

https://github.com/user-attachments/assets/4d5e171a-490c-4dbb-b246-62b66befcfef

# 2026 닷넷 개발자 데스크톱 개발

### WPF 실습

#### 카페 키오스크 개발

- 사용 스펙
  - WPF (.NET 10.0)
  - MaterialDesign (MaterialDesignInXamlToolKit)
  - MySQL + DBeaver

#### 프로젝트 생성

- WpfCafeKiosk
- NuGet Package, MaterialDesignThemes, MySQLConnector 설치
- MahApps.Metro.IconPacks 추가 설치

  ![](assets/20260608_101210_image.png)

### 프로젝트 구성

- WPF 머티리얼디자인 적용
- 키오스크 UI 제작
- 메뉴 모델, 주문 모델 생성
- 메뉴버튼 하드코딩
- MySQL menu 테이블 생성
- DB에서 메뉴 조회
- 메뉴버튼 동적 생성
- 주문목록, 총액 계산

#### MaterialDesign 적용

- App.xaml에 리소스딕셔너리 적용

#### MySQL DB, Table 생성

- cafekisok 데이터베이스 생성
- menu 테이블 생성
- orders, order_detail 테이블 생성

```sql
CREATE TABLE menu
(
    menu_id INT PRIMARY KEY AUTO_INCREMENT,
    menu_name VARCHAR(100) NOT NULL,
    price INT NOT NULL,
    image_path VARCHAR(255),
    category VARCHAR(20),
    is_sale CHAR(1) DEFAULT 'Y'
);

CREATE TABLE orders
(
    order_id INT PRIMARY KEY AUTO_INCREMENT,
    order_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    total_count INT NOT NULL,
    total_amount INT NOT NULL
);

CREATE TABLE order_detail
(
    detail_id INT PRIMARY KEY AUTO_INCREMENT,
    order_id INT NOT NULL,
    menu_id INT NOT NULL,
    menu_name VARCHAR(100) NOT NULL,
    price INT NOT NULL,
    count INT NOT NULL,
    total_price INT NOT NULL,
    CONSTRAINT fk_order_detail_orders
        FOREIGN KEY (order_id)
        REFERENCES orders(order_id)
);
```

![alt text](image-36.png)

#### 모델 클래스

- MenuItem - DB menu 테이블과 매핑
- OrderItem - 주문리스트 저장

#### 이미지 작업

* https://www.flaticon.com/
* https://icons.getbootstrap.com/
* Pixbay 등 사이트에서 다운로드
* 일부 편집
* Images 폴더에 붙여넣기
  ![alt text](image-33.png)

#### MainWindow UI 작업 및 기본 이벤트

![](assets/20260608_155106_image.png)

#### 기본 동작 이벤트 구현
https://github.com/user-attachments/assets/4382de8d-28e1-4b95-a121-24b8c30ffcb5


#### 메뉴 옵션 팝업창 작성

![alt text](image-34.png)

<<<<<<< Updated upstream
#### 실행 결과
https://github.com/user-attachments/assets/9832eaaf-8ef5-4c61-bffe-ffcf842fbce9

 
#### OpenAPI 연동앱 개발 
=======
#### 기본 동작 이벤트 구현

### 카페 키오스크 구현 리스트

- [X] 옵션 팝업창에서 수량 선택한 내용 주문담기 버튼 기능 구현
- [X] 키오스크 리스트뷰 음료 리스트업
- [X] 선택한 상품, 결제버튼 비용, 갯수 연동
- [X] 전체 삭제 기능
- [X] 남은 시간 완료 후 전체내용 초기화
- [X] 홈 버튼 클릭 초기화
- [X] 메인창에서 옵션창으로 MenuId 전달
- [ ] DB 연동: 메뉴 SELECT / 주문내역 INSERT
- [X] 메뉴 동적 바인딩

#### 옵션창 주문 내역 확인

![alt text](image-35.png)

- `Tag={Binding}` - 객체 자체 의미, OrderItem 객체 자체. 하위에서 MenuName, Count 등 사용 가능
- Margin, padding 위치 순서 - Left, Top, Right, Bottom / Left&Right, Top&Bottom
- CornerRadius 위치 순서 - TopLeft, TopRight, BottomLeft, BottomRight / TopLeft&BottomRight, TopRight&BottomLeft 순서


#### OpenAPI 연동앱 개발
>>>>>>> Stashed changes
