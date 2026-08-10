# 토이 프로젝트 5

## 컨베이어벨트 기반 스마트 공정관리 시스템

### 프로젝트 소개

아두이노, 라즈베리파이, MQTT, Unity를 활용하여 컨베이어벨트 기반 스마트 공정관리 시스템을 구현하였다.

공정에서 제품을 감지하고 센서 데이터를 수집하여 MQTT를 통해 실시간으로 전송하였으며, Unity 디지털 트윈과 연동하여 공정 현황을 시각화하였다. 또한 라즈베리파이와 시리얼 통신을 통해 아두이노 제어 및 데이터 수집을 수행하고, 스마트팩토리 공정관리 시스템의 전체 구조를 학습하였다.

### 주요 기능

- 컨베이어벨트 및 DC 모터 제어
- 적외선(IR) 센서를 이용한 제품 감지
- 서보모터를 이용한 제품 분류
- RGB LED 및 컬러센서를 활용한 상태 표시
- Arduino ↔ Raspberry Pi 시리얼 통신
- MQTT 기반 실시간 데이터 송수신
- Unity 디지털 트윈 모니터링
- 공정 데이터 실시간 시각화

### 기술 스택

`Arduino` · `C/C++` · `Raspberry Pi` · `Python` · `MQTT` · `Serial` · `Unity` · `C#` · `NeoPixel` · `Servo`

### 시스템 구성

```text
IR Sensor / Color Sensor
            │
      Arduino UNO
            │
      Serial Communication
            │
     Raspberry Pi (Python)
            │
          MQTT Broker
      ┌─────┴─────┐
      ▼           ▼
 Unity Digital   Monitoring
      Twin        System
```

### 빠른 이동

- [스마트팩토리](#스마트팩토리)
- [전체 시스템 구조](#전체-시스템-구조)
- [아두이노 컨베이어벨트](#아두이노-컨베이어벨트)
- [라즈베리파이 연결](#라즈베리파이-연결)
- [Unity 디지털트윈 시스템](#unity-디지털트윈-시스템)
- [WPF 모니터링 시스템](#wpf-모니터링-시스템)

---

## 컨베이어벨트 사용 공정관리 시스템

### 스마트팩토리

- 공장 내 모든 설비와 시스템을 연결, 데이터를 기반으로 생산을 최적화하는 제조 시스템

### 공장시스템 종류

- 회사내 다양한 종류 시스템(SW) 구성, 사용 중

|         시스템명         | 역할                            | 사용자                        |
| :-----------------------: | :------------------------------ | :---------------------------- |
|     SCM(공급체인관리)     | 원자재 구매, 협력업체, 물류관리 | 구매팀, 물류팀                |
|  `ERP(전사적자원관리)`  | 회사 전체 업무 관리(결과위주)   | 경영지원, 회계, 영업, 인사... |
|     MES(생산계획관리)     | 생산 현장 관리                  | 생산관리자                    |
|     PLC(생산로직제어)     | 기계 제어                       | 설비                          |
|           SCADA           | 설비모니터링                    | 생산현장                      |
| HMI(사람-기게 인터페이스) | 작업자 화면(터치패널)           | 작업자                        |
|       WMS(창고관리)       | 창고관리,재고관리               | 물류                          |
|       QMS(품질관리)       | 품질관리,품질계획관리           | 품질팀                        |
|    CMMS(유지보수관리)    | 설비 유지보수                   | 설비팀                        |

![alt text](image-376.png)

- 공정관리

  - MES의 한 파트인 공정(MRP:자재 소요 계획)을 실시간으로 모니터링, 제어
  - 스마트팩토리로 실시간으로 양품,불량을 선별 데이터 생성
  - Vision, IoT센서(적외선, X-ray, 스캐너...)
- IIoT - Industrial IoT. 대규모, 높은 정밀도, 고가...

### 전체 시스템 구조

![alt text](image-377.png)

### 아두이노 컨베이어벨트

#### 구성 요소

##### L298P 쉴드(HAT)

- 모터 드라이버를 포함한 아날로그 PWM, 디지털 GPIO를 구성한 쉴드
- 모터 드라이버 : 서보, DC등 모터를 쉽게 제어할 수 있도록 모듈화
- 모터 제어시 9V까지 전원 추가 - 아두이노 전원 불필요

![alt text](image-320.png)

- A - 디지털핀 13개
- B - 아날로그 확장 5개
- C - 아날로그핀 6개
- 확장핀 1 - PWM 확장핀, 5V, D6, D5, GND, D3(A와 공유)
- 확장핀 2 - 초음파센서 확장핀, 5V, D8, D7, GND
- 확장핀 3 - 서보모터 확장핀, GND, 5V, D9
- 확장핀 4 - 피에조 능동 부저, D4
- 확장핀 5 - 모터제어 포트, D13, D11, D12, D10 순

## 테스트

![alt text](image-378.png)

- Arduino IDE로 진행

![alt text](image-322.png)

- 부저 테스트

```cpp
int buzzer = 4;

void setup() {
  Serial.begin(9600);
  pinMode(buzzer, OUTPUT);
}

void loop() {
  digitalWrite(buzzer, HIGH);
  delay(1000);
  digitalWrite(buzzer, LOW);
  delay(2000);
}
```

- 기어드 DC 모터 컨베이어 테스트
  - L298P 쉴드에 최소 9V 전원(최대 24V)인가
  - 2A 넘기지 말 것

![alt text](image-379.png)

```cpp
int motorSpeedPin = 10;
int motorDirectionPin = 12;
int value;

void setup() {  
  pinMode(motorDirectionPin, OUTPUT);
  noTone(4);
}

void loop() {
  // 정방향
  digitalWrite(motorDirectionPin, HIGH);
  for (value = 0; value <= 255; value += 5) {
    analogWrite(motorSpeedPin, value);
    delay(30);
  }
  delay(1000);

  // 역방향
  digitalWrite(motorDirectionPin, LOW);
  for (value = 0; value <= 255; value += 5) {
    analogWrite(motorSpeedPin, value);
    delay(30);
  }
  delay(1000);
}
```

- 기어드 DC 모터 제어 - [소스](./toyproject/ToyProjects05/arduino_part/sample01/sample01.ino)

  - 모터 스피드 값 0 ~ 255 사이에서 제어, 실제 50이하는 동작안함
  - Default 80
  - 10부터 시작하면 60에서도 동작안함. 255에서 부터 줄여가면 50에서도 동작
- Serial Monitor 사용 주의점

  - 시리얼 입력에서 New Line, Carriage Return 선택, 입력하면 값 이외에 다른 데이터 전달됨

![alt text](image-380.png)

![alt text](image-382.png)

- 적외선 IR 송수신 센서

![alt text](image-381.png)

```cpp
// 적외선 IR 센서
int sensor = A0;
int val;

void setup() {
  Serial.begin(19200);
  pinMode(sensor, INPUT);
  Serial.println("Arduino start!");
}

void loop() {
  val = digitalRead(sensor);
  if (val == LOW) {
    Serial.println("Detected");
    delay(300);
  } else {
    Serial.println("0");
    delay(300);
  }
}
```

![alt text](image-383.png)

- 서보모터 SG-90
  - 확장핀 3 연결, 시그널 D9 전달
  - 각초 초기화 한 다음에 바를 연결

![alt text](image-384.png)

```cpp
// 서보모터
#include <Servo.h>
#define SERVO_PIN 9  // Digital 9
Servo servo;

void setup() {
  Serial.begin(19200);
  servo.attach(SERVO_PIN);  // 서보모터 연결
  servo.write(0);  // 0도로 초기화(!)
  delay(500);
}

void loop() {
  if (Serial.available()) {
    int value = Serial.parseInt();
    servo.write(value);
    Serial.println(value);
    delay(100);
  }
}
```

동영상 나중에

- RGB LED 네오픽셀
  - Adafruit NeoPixel 라이브러리 설치

![alt text](image-331.png)

```cpp
// NeoPixel LED 
#include <Adafruit_NeoPixel.h>
#define PIN 5
#define NUMPIXELS 3

Adafruit_NeoPixel pixels(NUMPIXELS, PIN, NEO_GRB + NEO_KHZ800);

void setup() {
  pixels.begin();
  pixels.setBrightness(50);
}

void loop() {
  for (int i=0; i < NUMPIXELS; i++) {
    pixels.setPixelColor(i, pixels.Color(255, 0, 0));
    pixels.show();
  }
  delay(1000);
  for (int i=0; i < NUMPIXELS; i++) {
    pixels.setPixelColor(i, pixels.Color(0, 255, 0));
    pixels.show();
    delay(10);
  }
  delay(1000);
  for (int i=0; i < NUMPIXELS; i++) {
    pixels.setPixelColor(i, pixels.Color(0, 0, 255));
    pixels.show();
    delay(10);
  }
  delay(1000);
}
```

- 1초당 RGB 색상 변경 확인
- 컬러센서(TCS34725) 모듈

  - RGB 색상 감지
  - Adafruit TCS34725 라이브러리 설치
    ![alt text](image-332.png)

```cpp
// Color Sensor
#include <Wire.h>
#include <Adafruit_TCS34725.h>

Adafruit_TCS34725 TCS = Adafruit_TCS34725(TCS34725_INTEGRATIONTIME_50MS, TCS34725_GAIN_4X);

void setup() {
  Serial.begin(19200);
  TCS.begin();  
}

void loop() {
  uint16_t clear, red, green, blue;
  delay(100);
  TCS.getRawData(&red, &green, &blue, &clear);

  int r = map(red, 0, 21504, 0, 2000);
  int g = map(green, 0, 21504, 0, 2000);
  int b = map(blue, 0, 21504, 0, 2000);

  Serial.print("    R: ");
  Serial.print(r);
  Serial.print("    G: ");
  Serial.print(g);
  Serial.print("    B: ");
  Serial.println(b);
}
```

![alt text](image-333.png)

- 색상 테스트
  - 초기상태 : RGB(4,3,3)
  - 파란색 물체 : RGB(8, 11, 15)
  - 녹색 물체 : RGB(14, 18, 10)
  - 빨간색 물체 : RGB(21, 6, 6)

#### 컨베이어벨트 조립

- 조립중간 단계

![alt text](image-334.png)

- 완성 단계

![alt text](image-385.png)

#### 통합로직 구현

- [전체소스](./toyproject/ToyProjects05/arduino_part/sortingmachine/sortingmachine.ino)

#### Arduino 교체 테스트

- [Arduino UNO R3](https://www.devicemart.co.kr/goods/view?no=34404) 에서 [Arduino UNO R4](https://www.devicemart.co.kr/goods/view?no=15088648)로 교체 테스트
- 결론 - `Adafruit` 등 라이브러리 UNO R4에서 사용불가

#### IR 적외선 센서팁

- 레일에 파란색, 검은색 전기테이프도 인식됨

#### 기본 동작

https://github.com/user-attachments/assets/f117f601-f956-414d-b6cc-8c420a34f4f7

### 라즈베리파이 연결

- 아두이노 + 라즈베리파이 5

![alt text](image-386.png)

#### MQTT 통신 구현

- Raspbian -> Windows MQTT 통신
- Python MQTT 기본통신 - [소스](./toyproject/ToyProjects05/raspberrypi_part/test_mqtt.py)
- 라즈베리파이 파이썬 실행상태

![alt text](image-387.png)

- 윈도우 MQTT 브로커 상태

![alt text](image-388.png)

#### 아두이노와 라즈베리파이 간 데이터 전달

1. 블루투스
2. `시리얼통신`
3. LAN 실드로 LAN선 연결

- 시리얼통신, 컨베이어 인식결과를 시리얼통신으로 전달 파이썬에서 확인
- 라즈베리파이에 연결된 시리얼 포트번호
  ![alt text](image-389.png)
- Python 시리얼 라이브러리 설치

```bash
> pip install pyserial
```

- 아두이노 시리얼 연결 테스트 - [소스](./toyproject/ToyProjects05/raspberrypi_part/test_serial.py)
- 시리얼 데이터 확인
  ![alt text](image-390.png)
- MQTT 소스 + 시리얼통신 소스 + 양방향 통신 - [소스](./toyproject/ToyProjects05/raspberrypi_part/data_interface.py)
- 통신 테스트

  - Arduino 컨베이어벨트 시작
  - RPi, Python 실행
  - MQTT Explorer
- 라즈베리파이

![alt text](image-391.png)

- Windows MQTT Explorer

![alt text](image-392.png)

### Unity 디지털트윈 시스템

- Unity 학습 시 사용한 ProductLine 재사용

![alt text](image-393.png)

- M2MqttUnity 설치
  - [Github](https://github.com/gpvigano/M2MqttUnity) 코드 다운로드
  - 압축해제 한 Assets 폴더를 Unity 프로젝트 Assets에 복사

![alt text](image-394.png)

- M2MqttUnity_Test 신 사용 테스트
- 접속 확인

![alt text](image-395.png)

- MQTT 데이터 Subscribe 로직 작성 - TOPIC에 맞춰서

#### Script 생성

- SmartFactoryMqttClient.cs 생성 및 작성

#### 빈 오브젝트 생성

- MqttClient
- Script 컴포넌트로 연결
- SensorTrigger.cs 의 로그 주석처리

#### 유니티 MQTT 토픽메시지 확인

![alt text](image-396.png)

#### Newtonsoft.Json 라이브러리 설치

- Window > Package Management > Package Manager
- `+` > Install Package by technical name 클릭
- `com.unity.nuget.newtonsoft-json` 설치

![alt text](image-397.png)

#### MQTTClient 스크립트 Json 처리

- Newtonsoft.Json 사용
- ProductResult.cs 스크립트 생성

#### Canvas 객체 추가

- 기본 2D를 3D로 변경
  - Canvas 컴포넌트 Render Mode를 `World Space`로 변경

![alt text](image-398.png)

- Rect Transform 조정

![alt text](image-399.png)

#### Panel 추가

- Rect Transform을 `Reset`
- Background 색상 변경

#### TMP 추가

- NanumGothic 폰트 임포트

![alt text](image-400.png)

#### 공정현황 텍스트들 추가

![alt text](image-401.png)

#### MQTTClient 스크립트 텍스트 바인딩

![alt text](image-402.png)

#### 실행결과

https://github.com/user-attachments/assets/49c4e7b5-6bfb-4b68-80e9-94a0d02e8349

- 미니프로젝트 3에서 진행

### 센싱결과 색상 표시(26.08.10)

- Unity에서 제품 변경된 색상 표시

#### 전체 시스템 연결 확인

- 내용 무

#### 컨베이어벨트 상 상품 인식

- 적외선 센서로 물체 감지를 MQTT로 전송

  - 적외선 센서 핀으로 LOW가 들어올 때까지 loop() 함수를 계속 빠져나가고 아두이노는 loop() 함수를 계속 실행

  ```cpp
  loop() {
    // 생략
    // 제품 적재여부 확인
    if (digitalRead(PIN_IR) == HIGH) return; // loop() 함수를 빠져나가지만 아두이노가 loop() 함수를 다시 실행

    // toneDetected();                     // 물체감지 사운드
    Serial.println("D");      // 물체감지했다는 값 전달
    // 생략
  }
  ```
- MQTT로 받은 물체 감지값을 Unity에서 확인

  - MQTT까지 데이터감지값(D)가 확인
  - D가 넘어왔을 때 Unity에서 처리
- 물체 감지 후 Box Spawner 실행 박스를 생성

  - BoxSpawner.cs에 시간별로 생성하는 로직을 일반 생성으로 변경
  - Timer 관련 소스 전부 삭제, 기본 생성만 유지

  ```cs
  public void Spawn()
  {
    Instantiate(prdPrefab,
                transform.position,
                Quaternion.identity);
    Debug.Log("Box 생성");
  }
  ```
- SensorTrigger.cs에 오류 발생. BoxSpwaner.cs 함수 제거로 수정
- Unity MQTT 스크립트 DecodeMessage() 메서드에 D에 대한 처리 추가

  ```cs
  [Header("Box Spawner")]
  public BoxSpawner boxSpawner; // MQTT에서 확인하고 박스를 생성

  // DecodeMessage() 메서드 내 추가
  if (prdResult.data == "D") {
    boxSpawner.Spawn(); // D가 들어올 때 박스 생성
  }
  ```
- 물체가 인식될 때마다 Unity 박스 생성 확인
- 이후 컨베이어벨트가 자동으로 움직임 - 동작연동 처리
- SensorTrigger.cs 수정

  - BoxSpawner 변수 삭제
  - 센서 위치에 박스가 들어오면 컨베이어를 멈추기만 함
  - 색상 판별 이후 컨베이어벨트 다시 동작하는 메서드 추가
  - yield return은 전부 제거(비동기 제거 -> 동기방식으로 HW와 연계)

  ```cs
  public class SensorTrigger : MonoBehaviour
  ```

{
[Header("컨베이어 2")]
public ConveyorBelt conveyor2;

```
//[Header("박스생성기")]
//public BoxSpawner spawner;

private bool isProcessing = false;

// 다른 Collider가 들어와서 Trigger 발생하면?
private void OnTriggerEnter(Collider other)
{
    if (isProcessing) return;

    if (other.CompareTag("Product"))
    {
        // 시간이 걸리는 작업을 여러 프레임에 나눠서 실행하는 기능
        //StartCoroutine(Process());  // 타이머로 처리 하지 안흠
        isProcessing = true;
        // 센서 위치에서 컨베이서 정지
        conveyor2.Stop();
        Debug.Log("제품 도착 - 색상판별 중");
    }
}

// MQTT에서 색상판별 완료 후 호출
public void Resume() {
    if (!isProcessing) return;
    conveyor2.StartBelt();
    isProcessing = false;
    Debug.Log("색상판별 완료");
}
```

}

```

- SmartFactoryMqttClient.cs에 SensorTrigger 연결

```cs
if (prdResult.data == "D") {
  boxSpwaner.Spawn();
} else if (prdResult.data == "R" ||
         prdResult.data == "G" ||
         prdResult.data == "B") {
  // 색상별로 박스 색상변경 추가

  sensorTrigger.Resume();
}
```

![](assets/20260810_111458_image.png)

- 칼라센서 결과로 유니티 박스 색상을 변경

  - 유니티 에디션에서 Red, Green, Blue 머티리얼 생성

  ![](assets/20260810_112633_image.png)

  - SensorTrigger.cs 스크립트에 색상 변경 메서드 SetColor
  - 머터리얼 연결 변수 추가
  -

  ```cs

  ```
- SmartFactoryMqttClient.cs에 색상 감지 후 SensorTrigger 색상변경 추가

  ```cs
  // 색상별로 박스 색상변경 추가
  sensorTrigger.SetColor(prdResult.data);

  ```

  ![](assets/20260810_114001_image.png)

#### Unity 실행결과


#### Unity 디지털트윈 소스 오픈

### ESP32-CAM 연동

### Database 데이터 저장

### ...

### WPF 모니터링 시스템

### 벨트 제어

- 모니터링 시스템에서 비상정지 기능
