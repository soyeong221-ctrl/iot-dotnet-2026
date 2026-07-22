## 동영상 물체인식
## 기본 OS운영
from io import BytesIO
import base64
import json

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

### 함수 선언
def detectObjects(image: np.array):
    # GPU(CUDA)가 있으면 FP16 추론을 quantize="fp16"로 지정한다.
    # half는 최신 Ultralytics에서 deprecated라서 quantize를 사용한다.
    # inference_mode로 불필요한 그래프 생성을 막아 메모리 사용량을 낮춘다.
    with torch.inference_mode():
        predict_args = {
            'imgsz': 320,
            'verbose': False,
            'device': 0 if IS_CUDA else 'cpu'
        }
        if IS_CUDA:
            predict_args['quantize'] = 'fp16'

        objects = model.predict(image, **predict_args)
    classNames = model.names

    for obj in objects:
        boxes = obj.boxes.xyxy
        confidences = obj.boxes.conf
        class_ids = obj.boxes.cls

        for (box, conf, class_id) in zip(boxes, confidences, class_ids):
            x1, y1, x2, y2 = map(int, box)
            label = classNames[int(class_id)]

            cv2.rectangle(image, (x1,y1), (x2,y2), (0,0,255), thickness=2)
            cv2.putText(image, f'{label} {conf:.2f}', (x1+7, y2-7), 
                        cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0,0,255), 2)

    return image

model = YOLO('yolov8m.pt')
IS_CUDA = torch.cuda.is_available()

# CV2 초기화
w, h = 640, 360
api = cv2.CAP_DSHOW   # Windows DirectX Show

# VideoCapture 시작 - 0:웹캠 또는 동영상 경로 
# 일반 동영상은 DirectShow 불가, 웹캠만 가능
cap = cv2.VideoCapture('./traffic_test.mp4')
# cap = cv2.VideoCapture(0, api)
# cap.set(cv2.CAP_PROP_FRAME_WIDTH, w) # 웹캠 Width 640 fix
# cap.set(cv2.CAP_PROP_FRAME_HEIGHT, h) # 웹캠 Height 360 고정

# 프레임 처리 간격 설정
# 매 프레임마다 YOLO 추론을 하지 않고 3프레임마다 한 번만 처리해서
# CPU/GPU 부하를 줄이고 전체 자원 사용량을 낮춘다.
frame_count = 0
last_result_img = None

while True:
    ret, frame = cap.read()

    if not ret:
        cap.set(cv2.CAP_PROP_POS_FRAMES, 0)  # 동영상 끝나면 처음으로 되돌림
        continue

    frame = cv2.resize(frame, (w, h))  # 동영상의 크기조절

    # 프레임을 매번 다 처리하지 않기:
    # 3프레임마다 한 번만 객체 인식을 수행하고,
    # 나머지 프레임은 직전 결과를 재사용한다.
    if frame_count % 3 == 0:
        resultImg = detectObjects(frame.copy())
        last_result_img = resultImg
    else:
        resultImg = last_result_img if last_result_img is not None else frame

    frame_count += 1

    # resultImg는 이미 numpy 배열이므로 다시 np.array로 복사하지 않는다.
    cv2.imshow('Result', resultImg)

    if cv2.waitKey(1) & 0xFF == ord('q'): break

# 리소스 해제
cap.release()
cv2.destroyAllWindows()