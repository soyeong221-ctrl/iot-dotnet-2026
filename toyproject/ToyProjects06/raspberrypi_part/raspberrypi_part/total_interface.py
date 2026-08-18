''' 
fileName : total_interface.py
description : 아두이노 + RPi + MQTT + ESP32-CAM YOLO 
lastdate : 2026-08-14 10:14
writer : Hugo MG Sung
'''

### 패키지 import
import cv2
import json
import time
import threading
from datetime import datetime

import serial
import paho.mqtt.client as mqtt
from ultralytics import YOLO

### MQTT 설정 
PUB_ID = 'IOT52-RPI'
BROKER = '210.119.12.52'
PORT = 1883

MQTT_USERNAME = 'root'
MQTT_PASSWORD = 'mqtt123456'

# Arduino -> Raspberry Pi -> Windows PUBLISH
DATA_TOPIC = 'smartfactory/52/data'
# Windows -> Raspberry Pi -> Arduino PUBLISH
CONTROL_TOPIC = 'smartfactory/52/control'

### Arduino Serial 설정
SERIAL_PORT = '/dev/ttyACM0'  # 컨베이어벨트 아두이노 연결 포트
BAUD_RATE = 19200

arduino = None
running = True

serial_lock = threading.Lock() 

### ESP32-CAM / YOLO 설정
URL = 'http://192.168.0.13/stream'  # ESP32-CAM streamming URL
# YOLO 모델
model = YOLO('best.pt')

# ROI 영역 설정
ROI_X1 = 130
ROI_Y1 = 250
ROI_X2 = 636
ROI_Y2 = 507

### Arduino 시리얼 데이터 전송 함수
def send_to_arduino(command: str):
    global arduino

    if arduino is None or not arduino.is_open:
        print('Arduino 시리얼포트 오픈 오류')
        return

    command = command.strip()  # ' D\n ' 앞뒤 공백 제거
    if not command: return

    try:
        with serial_lock:  # 데이터 전송하는 동안 스레드 잠궈줌
            arduino.write(f'{command}\n'.encode('utf-8'))

        print(f'[Serial TX] {command}')

    except serial.SerialException as error:
        print(f'Serial TX 에러 : {error}')

### MQTT Callback 함수들 묶음
def on_connect(client, userdata, flags, reason_code, properties=None):
    if reason_code == 0:
        print('MQTT 접속 성공')    

        client.subscribe(CONTROL_TOPIC, qos=1)  # 접속 후에 윈도우에서 전달하는 데이터 구독
        print(f'MQTT 구독 : {CONTROL_TOPIC}')
    else:
        print(f'MQTT 접속 실패 : {reason_code}')

def on_disconnect(client, userdata, disconnect_flags, reason_code, properties=None):
    print(f'MQTT 접속 종료 : {reason_code}')

def on_message(client, userdata, message):
    try:
        payload = message.payload.decode('utf-8').strip()

        print(f'[MQTT SUB] ' 
            f'topic={message.topic}, '
            f'payload={payload}'
        )        
        # MQTT로 전달받은 명령을 Arduino로 전송
        send_to_arduino(payload)

    except Exception as error:
        print(f'MQTT 메시지 에러 : {error}')

### Arduino Data -> MQTT Publish
def publish_arduino_data(client, serial_data: str):
    try:
        data = json.loads(serial_data)
    except json.JSONDecodeError:
        # R, G, B 등의 일반 문자열
        data = serial_data

    payload = {
        'deviceId': PUB_ID,
        'timestamp': datetime.now().isoformat(),
        'data': data
    }

    json_payload = json.dumps(
        payload,
        ensure_ascii=False
    )

    client.publish(
        DATA_TOPIC,
        payload=json_payload,
        qos=1
    )

    print(f'[MQTT PUB] {json_payload}')

### Arduino 시리얼통신 전달 데이터 스레드로 처리
def serial_receive_thread(client):
    global running
    global arduino

    while running:
        try:
            if arduino is not None and arduino.in_waiting > 0:
                with serial_lock:
                    serial_data = arduino.readline().decode(
                        'utf-8',
                        errors='ignore'
                    ).strip()

                if serial_data:
                    print(f'[Serial RX] {serial_data}')
                    #  MQTT Broker로 Publish
                    publish_arduino_data(client, serial_data)

            time.sleep(0.01)
        except serial.SerialException as error:
            print(f'Serial RX 에러 : {error}')
            time.sleep(1)
        except Exception as error:
            print(f'Serial Thread 에러 : {error}')
            time.sleep(1)

### ESP32-CAM YOLO 연결
def connect_camera():
    print('ESP32-CAM 연결....')
    cap = cv2.VideoCapture(URL)

    # 영상 버퍼 최소화
    cap.set(cv2.CAP_PROP_BUFFERSIZE, 1)
    return cap

### YOLO 물체인식 처리 함수
def yolo_process():
    global running

    cap = connect_camera()

    print('ESP32-CAM 연결 성공')
    print('YOLO 객체 인식 시작')

    while running:
        ret, frame = cap.read()

        # 영상 수신 실패
        if not ret:
            print('영상 수신 실패')
            cap.release()
            time.sleep(1)
            cap = connect_camera()
            continue

        # YOLO 객체 인식
        results = model(frame, imgsz=320, verbose=False)
        result_frame = frame.copy()

        # ROI 표시
        cv2.rectangle(
            result_frame,
            (ROI_X1, ROI_Y1),
            (ROI_X2, ROI_Y2),
            (255, 255, 255),
            2)

        # YOLO 검출 객체 확인
        for box in results[0].boxes:
            # 클래스 번호
            class_id = int(box.cls[0])
            # 클래스 이름
            class_name = model.names[class_id]
            # 신뢰도
            confidence = float(box.conf[0])
            # Bounding Box
            x1, y1, x2, y2 = map(int, box.xyxy[0])
            # 객체 중심 좌표
            center_x = (x1 + x2) // 2
            center_y = (y1 + y2) // 2

            # ROI 내부에 중심좌표가 들어간 객체만 처리
            if (ROI_X1 <= center_x <= ROI_X2
                and
                ROI_Y1 <= center_y <= ROI_Y2):
                # Bounding Box
                cv2.rectangle(result_frame,
                    (x1, y1),
                    (x2, y2),
                    (255, 0, 0),
                    2)

                # 클래스명 + confidence
                label = (f'{class_name} '
                    f'{confidence:.2f}')

                cv2.putText(
                    result_frame,
                    label,
                    (x1, y1 - 10),
                    cv2.FONT_HERSHEY_SIMPLEX,
                    0.7,
                    (255, 0, 0),
                    2)

                # 중심점
                cv2.circle(
                    result_frame,
                    (center_x, center_y),
                    5,
                    (255, 255, 255),
                    -1)

                print(
                    f'[YOLO ROI] '
                    f'{class_name} '
                    f'{confidence:.2f}'
                )

        # 영상 출력
        cv2.imshow('ESP32-CAM YOLO', result_frame)
        # q 입력 시 종료
        if cv2.waitKey(1) & 0xFF == ord('q'):
            running = False
            break

    cap.release()
    cv2.destroyAllWindows()

### Main 함수
def main():
    global arduino
    global running
    client = None

    try:
        # Arduino 연결
        arduino = serial.Serial(
            port=SERIAL_PORT,
            baudrate=BAUD_RATE,
            timeout=1)

        # Arduino Reset 대기
        time.sleep(2)
        arduino.reset_input_buffer()

        print(f'Arduino 연결 : '
            f'{SERIAL_PORT}')

        # MQTT Client 연결
        client = mqtt.Client(
            client_id=PUB_ID,
            protocol=mqtt.MQTTv5,
            userdata=None
        )

        client.username_pw_set(
            username=MQTT_USERNAME,
            password=MQTT_PASSWORD
        )

        # 콜백함수 설정
        client.on_connect = on_connect
        client.on_disconnect = on_disconnect
        client.on_message = on_message
        client.connect(
            BROKER,
            PORT,
            keepalive=60
        )
        client.loop_start()  # 무한 반복
        print(
            f'Publish Topic : '
            f'{DATA_TOPIC}'
        )

        print(
            f'Subscribe Topic : '
            f'{CONTROL_TOPIC}'
        )

        # Arduino Serial Thread 시작
        serial_thread = threading.Thread(
            target=serial_receive_thread,
            args=(client,),
            daemon=True
        )
        serial_thread.start()

        # YOLO 실행
        yolo_process()

    except KeyboardInterrupt:
        print('\n프로그램 종료')
        running = False

    except serial.SerialException as error:
        print(
            f'Serial 에러 : '
            f'{error}'
        )
    except Exception as error:
        print(
            f'에러 : '
            f'{error}'
        )
    finally:
        running = False

        # MQTT 종료
        if client is not None:
            client.loop_stop()
            client.disconnect()

        # Arduino Serial 종료
        if (arduino is not None
            and
            arduino.is_open):
            arduino.close()

        cv2.destroyAllWindows()
        print('프로그램 종료!')

### 프로그램 실행
if __name__ == '__main__':
    main()