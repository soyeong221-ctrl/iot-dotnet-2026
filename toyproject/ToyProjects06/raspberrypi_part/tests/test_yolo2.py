import cv2
import time
from ultralytics import YOLO

# ESP32-CAM 주소
URL = 'http://192.168.0.13/stream'  # 기본웹 80포트, stream 81포트

# YOLO 모델
model = YOLO('best.pt')

# ROI 영역 설정
# x1, y1
ROI_X1 = 130
ROI_Y1 = 250

# x2, x2
ROI_X2 = 636
ROI_Y2 = 507

def connect():
    print('ESP32-CAM 연결....')
    cap = cv2.VideoCapture(URL)
    # 버퍼 최소화
    cap.set(cv2.CAP_PROP_BUFFERSIZE, 1)

    return cap

cap = connect()
print('ESP32-CAM 연결 성공')
print('YOLO 객체 인식 시작')


while True:
    ret, frame = cap.read()

    if not ret:
        print('영상 수신 실패')
        cap.release()
        time.sleep(1)
        cap = connect()
        continue

    # YOLO 객체 인식
    # imgsz=320 입력이미지를 320으로 픽셀 줄이기
    # roi = frame[ROI_Y1:ROI_Y2, ROI_X1:ROI_X2]
    results = model(frame, imgsz=320, verbose=False)

    # 결과 이미지
    # result_frame = results[0].plot()
    result_frame = frame.copy()

    # ROI 영역 표시
    cv2.rectangle(
        result_frame,
        (ROI_X1, ROI_Y1),
        (ROI_X2, ROI_Y2),
        (255, 255, 255),
        2
    )

    # YOLO 검출 객체 확인
    for box in results[0].boxes:
        # 클래스 번호
        class_id = int(box.cls[0])
        # 클래스 이름
        class_name = model.names[class_id]
        # 정확도
        confidence = float(box.conf[0])
        # Bounding Box
        x1, y1, x2, y2 = map(int, box.xyxy[0])

        # 객체 중심 좌표
        center_x = (x1 + x2) // 2
        center_y = (y1 + y2) // 2

        # ROI 내부 객체만 처리
        if (ROI_X1 <= center_x <= ROI_X2 and ROI_Y1 <= center_y <= ROI_Y2):
            # Bounding Box
            cv2.rectangle(result_frame, (x1, y1), (x2, y2), (255, 0, 0), 2)

            # 클래스명 + confidence
            label = f'{class_name} {confidence:.2f}'

            cv2.putText(result_frame, label, (x1, y1 - 10), 
                        cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255, 0, 0), 2 )

            # 중심점
            cv2.circle(result_frame, (center_x, center_y), 5, (255, 255, 255), -1)
            print(f'ROI 감지: {class_name} {confidence:.2f}')

    cv2.imshow('ESP32-CAM YOLO', result_frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()