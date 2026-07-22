# FastAPI Service 05 - Again
## WebService를 위한 패키지
import uvicorn
from fastapi import FastAPI, HTTPException, UploadFile, File, Form
from fastapi.responses import FileResponse, StreamingResponse
from pydantic import BaseModel

## 기본 OS운영
from io import BytesIO
import base64

## 이미지처리
from PIL import Image, ImageDraw, ImageFont

## 영상처리, 물체인식 패키지
import numpy as np
import cv2
from ultralytics import YOLO
from ultralytics.engine.results import Results

# 파일명과 동일하게 main## 지정!
appName = 'main05:app'

## 웹서비스
app = FastAPI()

## YOLO 실행
model = YOLO('yolov8m.pt')  # YOLO v8 pretrained model

## 웹상으로 전달할 결과클래스
class DetectionResult(BaseModel):
    message: str        # 객체 인식 결과메시지
    image: str          # 인식결과 이미지

## 이미지 객체탐지 함수
def detectObjects(image: Image.Image):
    origin = np.array(image)    # PILLOW이미지를 numpy 배열로 변환
    # 인식후 리턴타입 list[urltralitics.engine.results.Results]
    objects = model(origin)     # YOLO에 담아서 물체인식 처리
    classNames = model.names    # person, clock, car... 등 클래스명

    # 이미지에 객체영역 그리기 처리
    for obj in objects:         # 인식된 물체 개수만큼 반복
        boxes = obj.boxes.xyxy              # 박스의 위치
        confidences = obj.boxes.conf        # 박스의 인식된 물체의 신뢰도
        class_ids = obj.boxes.cls           # 박스 인식된 물체 클래스 번호

        for (box, conf, class_id) in zip(boxes, confidences, class_ids):
            # if conf <= 0.5: continue

            x1, y1, x2, y2 = map(int, box)      # x1,y1(박스 좌측상단), x2,y2(박스 우측하단)
            label = classNames[int(class_id)]   # 단일클래스 명

            # 인식된 객체 사각형 그리기
            cv2.rectangle(origin, (x1,y1), (x2,y2), (255,0,0), thickness=2)
            cv2.putText(origin, f'{label} {conf:.2f}', (x1+7, y2-7), 
                        cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255,0,0), 2)

    result = Image.fromarray(origin)        # 결과 다시 PILLOW 변환
    return result

@app.get('/')
async def root():
    image = Image.open('./test01.png')  # PILLOW 패키지로 이미지오픈 메모리 업

    if image.mode != 'RGB': image = image.convert('RGB')

    result = detectObjects(image)       # YOLO로 물체인식, 영역 표시

    buffer = BytesIO()
    result.save(buffer, format='PNG')
    buffer.seek(0)

    return StreamingResponse(buffer, media_type='image/png')

# ASP.NET Core Website에서 form 양식에서 넘어온 메시지와 이미지로 객체 인식
# 결과를 다시 Response 해주는 함수
@app.post('/detect', response_model=DetectionResult)
async def detectService(message: str=Form(...), file: UploadFile = File(...)):
    message = '[서버처리완료] ' + message

    # 이미지 로드 PILLOW이미지로 변환
    image = Image.open(BytesIO(await file.read())) # 웹으로 전달된 이미지 로드

    # RGB로 변환
    if image.mode != 'RGB': image = image.convert('RGB')

    # 객체탐지
    result = detectObjects(image)

    # 이미지결과 Base64 스트링 인코딩(웹상으로 전달)
    buffer = BytesIO()
    result.save(buffer, format='JPEG')
    buffer_str = base64.b64encode(buffer.getvalue()).decode('utf-8')

    return DetectionResult(message=message, image=buffer_str)

if __name__ == '__main__':
    uvicorn.run(app=appName, reload=True, port=8080)