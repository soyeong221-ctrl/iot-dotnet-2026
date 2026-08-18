import cv2

URL = "http://192.168.0.13/stream"

cap = cv2.VideoCapture(URL)   # URL로 접근 가능

if not cap.isOpened():
    print('ESP32-CAM 연결 실패')
    exit()

print('ESP32-CAM 연결 성공')

while True:
    ret, frame = cap.read()

    if not ret:
        print('영상 수신 실패')
        break

    cv2.imshow('ESP32-CAM', frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()

