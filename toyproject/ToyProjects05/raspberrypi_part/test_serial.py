# test_serial.py
import time
import serial

PORT = '/dev/ttyACM0'
BAUD_RATE = 19200

try:
    arduino = serial.Serial(
                    port=PORT,
                    baudrate=BAUD_RATE,
                    timeout=1
                    )

    # serial connection delay time
    time.sleep(2)

    print(f'Arduino connection succeed: {PORT}')

    while True:
        if arduino.in_waiting > 0:
            data = arduino.readline().decode('utf-8').strip()

            if data:
                print(f'Arduino data : {data}')


except serial.SerialException as error:
    print(error)

except KeyboardInterrupt:
    print("Exit");

finally:
    if 'arduino' is locals() and arduino.is_open:
        arduino.close()