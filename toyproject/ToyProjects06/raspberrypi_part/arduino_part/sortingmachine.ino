/// 스마트팩토리 공정관리용 컨베이어벨트 
/**
  요소
  1. 기어드 DC모터 : 레일동작
  2. 서보머터 : 제품 색상따라 분류
  3. 적외선 IR센서 : 물품 감지
  4. 컬러센서 : 색상감지
  5. RBG LED : 감지된 색상 표시

  동작순서
  1. 컨베이어벨트 작동
  2. IR센서에서 제품 감지되면 일시 정지
  3. 컬러센서까지 이동
  4. 컬러센서에서 색상감지
  5. R, G, B가 결정되면 서보모터 각도 조절 회전
  6. 레일 재가동 물체 분류
*/

// 라이브러리
#include <Wire.h>                   // I2C 통신용
#include <Servo.h>                  // 서보모터
#include <Adafruit_NeoPixel.h>      // LED 모듈
#include <Adafruit_TCS34725.h>      // 컬러 센서

// 상수 선언부
#define PIN_DC_DIRECTION    13      // DC모터 방향을 정하는 핀
#define PIN_DC_SPEED        11      // DC모터 속도를 정하는 핀
#define PIN_SERVO           9       // 서보모터 핀
#define PIN_LED             5       // LED 핀
#define PIN_IR              A0      // 적외선 IR센서 핀

#define DGR_RED             40      // 빨간색 서보모터 각도
#define DGR_GREEN           57      // 초록색 서보모터 각도
#define DGR_BLUE            2       // 파란색 서보모터 각도
#define NUM_PIXELS          3       // 네오픽셀 수, RGB

// 변수 선언부
Servo servo;
Adafruit_TCS34725 TCS = Adafruit_TCS34725(TCS34725_INTEGRATIONTIME_50MS, TCS34725_GAIN_4X);
Adafruit_NeoPixel pixels(NUM_PIXELS, PIN_LED, NEO_GRB + NEO_KHZ800);

uint16_t clear, red, green, blue;   // 색상 값을 저장할 unsigned short int형 변수
int r, g, b, sum;                   // 색상 값을 사용하기 위한 변환값
int railSpeed = 100;                 // 레일 기본 속도, 초기값은 160. 12V로는 70가능. 9V일때는 80이하 불가능
char color = 'N';                   // 확인 색상 (기본 N)

// 초기화
void setup() {
  Serial.begin(19200);
  // DC 모터 설정
  pinMode(PIN_DC_DIRECTION, OUTPUT);    // DC모터 방향핀 OUTPUT(동작)으로 설정
  digitalWrite(PIN_DC_DIRECTION, HIGH); // 방향 정방향
  analogWrite(PIN_DC_SPEED, railSpeed); // 레일 동작

  servo.attach(PIN_SERVO);              // 서보모터 연결
  servo.write(10);                       // 초기 각도 재설정. 최초연결시 바 각도마다 다르므로 따로 설정
  delay(500);                           
  servo.detach();                       // 서보모터 연결분리

  // IR센서 설정
  pinMode(PIN_IR, INPUT);               // 적외선 센서는 INPUT(읽어오니까)

  // 컬러센서 설정
  TCS.begin();

  // 네오픽셀 설정
  pixels.begin();
  pixels.setBrightness(255);            // 최대 밝기
  pixels.show();
}

// 반복작업
void loop() {
  // Raspberry Pi에서 시리얼 데이터 수신
  if (Serial.available() > 0) {
    char command = Serial.read();

    // 개행문자 무시
    if (command == '\n' || command == '\r') return;

    processSerialCommand(command);
    return;
  }
  // analogWrite(PIN_DC_SPEED, 0);   // 벨트끄기. 나중에 주석 처리

  // 제품 적재여부 확인
  if (digitalRead(PIN_IR) == HIGH) return; // loop() 함수를 빠져나가지만 아두이노가 loop() 함수를 다시 실행

  // toneDetected();                     // 물체감지 사운드
  Serial.println("D");      // 물체감지했다는 값 전달
  analogWrite(PIN_DC_SPEED, railSpeed - 20); 
  // 일정 시간 후 멈추기
  delay(600);                         // 500ms 딜레이
  analogWrite(PIN_DC_SPEED, 0);       // 컬러센서에서 레일 정지

  // 컬러센서로 색상판별
  do {
    TCS.getRawData(&red, &green, &blue, &clear);      // 색상센서 동작
    r = map(red, 0, 21504, 0, 1000);
    g = map(green, 0, 21504, 0, 1000);
    b = map(blue, 0, 21504, 0, 1000);
    sum = r + g + b;
  } while (sum < 15);

  toneDetected();
  delay(1000);    // 1초 딜레이
  color = getColor(r, g, b);
  Serial.println(color);  

  // 색상에 따른 분류 
  setProductColor(color);   

  analogWrite(PIN_DC_SPEED, railSpeed);
  delay(1000);
}

// 적외선 센서, 색상감지 등 물체 감지시 소리출력
void toneDetected() {
  tone(4, 523, 50);                     // 도. 0.05초간 출력
  delay(100);
  tone(4, 784, 50);                     // 미, 0.05초간 출력
  delay(100);
}

char getColor(int r, int g, int b) {
  if (r < 5 && g < 5 && b < 5) {
    return 'N';
  } else if (r > g && r > b) {
    return 'R';
  } else if (g > r && g > b) {
    return 'G';
  } else if (b > r && b > g) { 
    return 'B';
  }
}

// RPi 명령 처리
void processSerialCommand(char cmd) {
  // 소문자가 들어와도 처리
  if (cmd == 'r') cmd = 'R';
  if (cmd == 'g') cmd = 'G';
  if (cmd == 'b') cmd = 'B';


  if (cmd == 'R' || cmd == 'G' || cmd == 'B') {
    color = cmd;

    // 컨베이어 정지
    analogWrite(PIN_DC_SPEED, 0);

    toneDetected();
    // 서보모터 + LED 제어
    setProductColor(color);
    // 약간 대기
    delay(500);
    // 컨베이어 재가동
    analogWrite(PIN_DC_SPEED, railSpeed);

    //Serial.print("YOLO COLOR : "); // MQTT 로 전송안함
    Serial.println(color); // RPi 로 전달하면 MQTT 배포 예상
  }
}

// 색상에 따른 서보모터, LED 처리
void setProductColor(char color) {
  
  servo.attach(PIN_SERVO);  // 서버모터 사용하려면 연결

  if (color == 'R') {
    servo.write(DGR_RED);
    pixels.setPixelColor(0, pixels.Color(255, 0, 0) );
    pixels.setPixelColor(1, pixels.Color(255, 0, 0) );
    pixels.setPixelColor(2, pixels.Color(255, 0, 0) );
  }  
  else if (color == 'G') {
    servo.write(DGR_GREEN);
    pixels.setPixelColor(0, pixels.Color(0, 255, 0) );
    pixels.setPixelColor(1, pixels.Color(0, 255, 0) );
    pixels.setPixelColor(2, pixels.Color(0, 255, 0) );
  }
  else if (color == 'B') {
    servo.write(DGR_BLUE);
    
    pixels.setPixelColor(0, pixels.Color(0, 0, 255) );
    pixels.setPixelColor(1, pixels.Color(0, 0, 255) );
    pixels.setPixelColor(2, pixels.Color(0, 0, 255) );
  }

  // LED 출력
  pixels.show();
  // 서보 이동 시간
  delay(500);
  servo.detach();
}
