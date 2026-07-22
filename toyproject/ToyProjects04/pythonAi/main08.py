## 웹캠 물체인식
## 기본 OS운영
from io import BytesIO
import base64
import json
import time

## 이미지처리
# from PIL import Image, ImageDraw, ImageFont

## 영상처리, 물체인식 패키지
import numpy as np
import cv2
import torch
from ultralytics import YOLO
from ultralytics.engine.results import Results

## MQTT
import paho.mqtt.client as mqtt

### 웹소켓 설정
host = '127.0.0.1'  # localhost / 외부로 접속하려면 아이피 변경
port = 9001     # 웹소켓
topic = 'visionai/objectdetects'   # 토픽

### MQTT Client 객체 생성
client = mqtt.Client(
            client_id='RealTimeVision',
            transport='websockets',
            protocol=mqtt.MQTTv5,
            userdata=None
         )

### MQTT 계정, 비밀번호 추가
client.username_pw_set(
    username='root',
    password='mqtt123456'
)

### MQTT 접속성공 이벤트
def onConnect(client, userdata, flags, reason_code, properties=None):
    print(f'[MQTT] 연결성공. reason={reason_code}')

### MQTT 메시지 이벤트
def onMessage(client, userdata, msg):
    print(msg.topic, msg.payload.decode())

### MQTT 실제 접속처리
client.on_connect = onConnect
client.on_message = onMessage
client.connect(host=host, port=port, keepalive=60)
client.loop_start()

## 함수 선언
### 색상설정 함수
def getColors(num_colors):
    np.random.seed(42)
    colors = [tuple(np.random.randint(0,255,3).tolist()) for _ in range(num_colors)]

    return colors


model = YOLO('yolov8m.pt')

# 색상표
class_names = model.names   # YOLOv8m 대략 80개
num_classes = len(class_names)
colors = getColors(num_classes)

### 물체인식 함수
def detectObjects(image: np.array):
    objects = model(image)
    classNames = model.names

    for obj in objects:
        boxes = obj.boxes.xyxy
        confidences = obj.boxes.conf
        class_ids = obj.boxes.cls

        for (box, conf, class_id) in zip(boxes, confidences, class_ids):
            x1, y1, x2, y2 = map(int, box)
            label = classNames[int(class_id)]

            # 클래스별로 색상 변경
            cv2.rectangle(image, (x1,y1), (x2,y2), colors[int(class_id)], thickness=2)
            cv2.putText(image, f'{label} {conf:.2f}', (x1+7, y2-7), 
                        cv2.FONT_HERSHEY_SIMPLEX, 0.5, colors[int(class_id)], 2)

    return image


# CV2 초기화
w, h = 640, 360
# api = cv2.CAP_DSHOW   # Windows DirectX Show

# VideoCapture 시작 - 0:웹캠 또는 동영상 경로 
# 일반 동영상은 DirectShow 불가, 웹캠만 가능
cap = cv2.VideoCapture(0)

## Logitech 웹캠에서 해상도 문제로 프레임캡쳐 실패 발생
# cap.set(cv2.CAP_PROP_FRAME_WIDTH, w) # 웹캠 Width 640 fix
# cap.set(cv2.CAP_PROP_FRAME_HEIGHT, h) # 웹캠 Height 360 고정
## OpenCV 윈도우창 사이즈 조정하면 오류발생 안함
cv2.namedWindow('Result', cv2.WINDOW_NORMAL)
cv2.resizeWindow('Result', w, h)

## FPS 표시
frame_count = 0
fps = 0
start_time = time.time()

while cap.isOpened():
    ret, frame = cap.read()

    if not ret: break

    # FPS 계산
    frame_count += 1
    elapsed = time.time() - start_time

    if elapsed >= 1.0:
        fps = frame_count / elapsed
        frame_count = 0
        start_time = time.time()

    frame = cv2.resize(frame, (w, h))  # 동영상의 크기조절
    resultImg = detectObjects(frame)

    # resultImg에 FPS 출력
    cv2.putText(resultImg, f'FPS : {fps:.1f}', (10,30), 
                cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 255, 0), 2)

    # resultImg numpy.array 형태로 base64 인코딩 전달필요
    _, buffer = cv2.imencode('.jpg', resultImg)
    jpgStr = base64.b64encode(buffer).decode('utf-8')

    # MQTT 웹소켓으로 전송
    payload = json.dumps({ 'image': jpgStr })
    client.publish(topic=topic, payload=payload)

    # resultImg는 이미 numpy 배열이므로 다시 np.array로 복사하지 않는다.
    cv2.imshow('Result', resultImg)

    if cv2.waitKey(1) & 0xFF == ord('q'): break

# 리소스 해제
cap.release()
cv2.destroyAllWindows()