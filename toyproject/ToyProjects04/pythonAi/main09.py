## 화재감지 테스트용
import base64
import json
import time

import numpy as np
import cv2
from ultralytics import YOLO

import paho.mqtt.client as mqtt

host = '127.0.0.1'
port = 9001
topic = 'visionai/firedetects'

client = mqtt.Client(
            client_id='RealTimeVision',
            transport='websockets',
            protocol=mqtt.MQTTv5,
            userdata=None
         )

client.username_pw_set(
    username='root',
    password='mqtt123456'
)

def onConnect(client, userdata, flags, reason_code, properties=None):
    print(f'[MQTT] 연결성공. reason={reason_code}')

def onMessage(client, userdata, msg):
    print(msg.topic, msg.payload.decode())

client.on_connect = onConnect
client.on_message = onMessage
client.connect(host=host, port=port, keepalive=60)
client.loop_start()

model = YOLO('yolov11_firedetect.pt')  # 화재감지 모델

w, h = 640, 360
api = cv2.CAP_DSHOW 

cap = cv2.VideoCapture('./sample_fire.mp4')

cv2.namedWindow('Result', cv2.WINDOW_NORMAL)
cv2.resizeWindow('Result', w, h)

frame_count = 0
fps = 0
start_time = time.time()

while True:
    ret, frame = cap.read()

    if not ret:
        cap.set(cv2.CAP_PROP_POS_FRAMES, 0)
        continue

    frame_count += 1
    elapsed = time.time() - start_time

    if elapsed >= 1.0:
        fps = frame_count / elapsed
        frame_count = 0
        start_time = time.time()

    cv2.putText(frame, f'FPS : {fps:.1f}', (10,25), 
                cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 255, 0), 2)
    
    # 모델에 대입해서 60% 이상 확률일 때만 인식
    results = model(frame, conf=0.6)
    detected = False

    # 결과에 불러온 박스를 반복 확인
    for box in results[0].boxes:
        cls_id = int(box.cls[0])
        label = model.names[cls_id]

        if label in ['Fire', 'Smoke']:
            # 화재/연기 감지
            detected = True

    # 결과 시각화한 프레임 리턴
    resultImg = results[0].plot()

    _, buffer = cv2.imencode('.jpg', resultImg)
    jpgStr = base64.b64encode(buffer).decode('utf-8')

    # 화재/연기 감지 여부 json 전달
    payload = json.dumps({ 'detect': detected,'image': jpgStr })
    client.publish(topic=topic, payload=payload)

    cv2.imshow('Result', resultImg)

    if cv2.waitKey(1) & 0xFF == ord('q'): break

cap.release()
cv2.destroyAllWindows()
