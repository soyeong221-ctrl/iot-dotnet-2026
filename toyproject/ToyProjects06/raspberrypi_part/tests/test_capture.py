import os

# Qt 관련 경고 최소화
# 라즈비안 Wormbook 버전까지와 Trixie 버전 Qt 라이브러리 코어 상이
# OpenCV Window(Qt기반)이 Trixie라는 것을 지정
os.environ["QT_QPA_PLATFORM"] = "xcb"

import cv2
import requests
import numpy as np

STREAM_URL = "http://192.168.0.13/stream"
SAVE_DIR = "captures"

os.makedirs(SAVE_DIR, exist_ok=True)

# ============================================================
# 파일 번호 찾기
# ============================================================
capture_count = 1

while os.path.exists(
    os.path.join(
        SAVE_DIR,
        f"capture_{capture_count:03d}.jpg"
    )
):
    capture_count += 1

# ============================================================
# ESP32-CAM 연결
# ============================================================
print("ESP32-CAM 연결 중...")

response = requests.get(
    STREAM_URL,
    stream=True,
    timeout=(5, None)
)

response.raise_for_status()

print("ESP32-CAM 연결 성공")
print("S : 이미지 저장")
print("Q : 종료")

# ============================================================
# raw 스트림
# ============================================================
stream = response.raw
buffer = bytearray()

try:
    while True:
        # ----------------------------------------------------
        # HTTP raw 데이터 읽기
        # ----------------------------------------------------
        chunk = stream.read(1024)

        if not chunk:
            print("스트림 연결 종료")
            break

        buffer.extend(chunk)

        # ----------------------------------------------------
        # JPEG 시작 위치
        # FF D8
        # ----------------------------------------------------
        start = buffer.find(b'\xff\xd8')

        if start == -1:
            continue
        # ----------------------------------------------------
        # JPEG 끝 위치
        # FF D9
        # ----------------------------------------------------
        end = buffer.find(
            b'\xff\xd9',
            start
        )

        if end == -1:
            continue

        # ----------------------------------------------------
        # JPEG 하나 추출
        # ----------------------------------------------------
        jpg = bytes(
            buffer[start:end + 2]
        )

        # 사용한 부분 제거
        del buffer[:end + 2]

        # ----------------------------------------------------
        # JPEG → OpenCV 이미지
        # ----------------------------------------------------
        image_array = np.frombuffer(
            jpg,
            dtype=np.uint8
        )

        frame = cv2.imdecode(
            image_array,
            cv2.IMREAD_COLOR
        )

        if frame is None:
            continue

        # ====================================================
        # 화면 출력
        # ====================================================
        cv2.imshow(
            "ESP32-CAM",
            frame
        )

        # ====================================================
        # 키 입력
        # ====================================================
        key = cv2.waitKey(1) & 0xFF

        # ----------------------------------------------------
        # S : 이미지 저장
        # ----------------------------------------------------
        if key == ord('s') or key == ord('S'):
            filename = (
                f"capture_{capture_count:03d}.jpg"
            )
            filepath = os.path.join(
                SAVE_DIR,
                filename
            )

            cv2.imwrite(
                filepath,
                frame
            )

            print(
                f"저장 완료 : {filepath}"
            )

            capture_count += 1

        # ----------------------------------------------------
        # Q : 종료
        # ----------------------------------------------------
        elif key == ord('q') or key == ord('Q'):
            print("프로그램 종료")
            break

except Exception as e:
    print("스트림 오류:", e)
finally:
    response.close()
    cv2.destroyAllWindows()