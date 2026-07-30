# data_interface.py 
# arduino, raspberry pi, windows interface python code

import json
import time
import threading
from datetime import datetime

import serial
import paho.mqtt.client as mqtt

## MQTT Init
PUB_ID = 'IOT52-RPI'
BROKER = '210.119.12.52'     # 본인 아이피
PORT = 1883

MQTT_USERNAME = 'root'
MQTT_PASSWORD = 'mqtt123456'

# publish topic : Arduino -> RPI -> WIN
DATA_TOPIC = 'smartfactory/52/data'

# subcribe topic : WIN -> RPI -> Arduino
CONTROL_TOPIC = 'smartfactory/52/control'

## Serial Communication Init
SERIAL_PORT = '/dev/ttyACM0'
BAUD_RATE = 19200

arduino = None
running = True
serial_lock = threading.Lock()

## Data transfer to Arudino
def send_to_arduino(command: str):
    # from MQTT command to Aruino
    if arduino is None or not arduino.is_open:
        print('Arduino seral port is not open')
        return

    command = command.strip()
    if not command: return

    arduino.write(f'{command}\n'.encode('utf-8'))
    print(f'[Serial TX] {command}')    

## MQTT onMethod events
# Connection
def on_connect(client, userdata, flags, reason_code, properties=None):
    if reason_code == 0:
        print('MQTT connected')

        client.subscribe(CONTROL_TOPIC, qos=1)        
        print(f'MQTT subscribed: {CONTROL_TOPIC}')
    else:
        print('MQTT connection failed')

# Disconnection
def on_disconnect(client, userdata, disconnect_flags, reason_code, properties=None):
    print(f'MQTT disconnected: {reason_code}')

# Message receive
def on_message(client, userdata, message):
    # TODO
    print(f'[MQTT Sub] topic={message}')

    # command process
    command = ''
    # 
    send_to_arduino(str(command))

# Publish Ardino data by MQTT
def publish_arduino_data(client, serial_data: str):
    try:
        data = json.loads(serial_data)

    except json.JSONDecodeError:
        # R, G, B plain text
        data = serial_data

    payload = {
        'deviceId': PUB_ID,
        'timestamp': datetime.now(),
        'data': data
    }

    json_payload = json.dumps(
        payload,
        ensure_ascii=False
    )

    client.publish(DATA_TOPIC, payload=json_payload, qos=1)
    print(f'[MQTT PUB] {json_payload}')


### Main function
def main():
    global arduino
    client = None

    try:
        # Arduino connect
        arduino = serial.Serial(
            port=SERIAL_PORT,
            baudrate=BAUD_RATE,
            timeout=1
        )

        # reboot need
        time.sleep(2)
        arduino.reset_input_buffer()
        print(f'Arduino connected : {SERIAL_PORT}')

        # MQTT client 
        client = mqtt.Client(client_id=PUB_ID, protocol=mqtt.MQTTv5, userdata=None)
        client.username_pw_set(username=MQTT_USERNAME, password=MQTT_PASSWORD)

        client.on_connect = on_connect
        client.on_disconnect = on_disconnect
        client.on_message = on_message

        client.connect(BROKER, PORT, keepalive=60)
        client.loop_start()

        print(f'Publish Topic : {DATA_TOPIC}')
        print(f'Subscribe Topic : {CONTROL_TOPIC}')

        # Arduino Data receive
        while True:
            if arduino.in_waiting > 0:
                serial_data = arduino.readline().decode(
                    'utf-8',
                    errors='ignore'
                ).strip()

                if serial_data:
                    print(f'[Serial RX] {serial_data}')
                    # MQTT publish
                    publish_arduino_data(client, serial_data)

            time.sleep(0.01)

    except KeyboardInterrupt:
        print('\nProgram quit')

    except serial.SerialException as error:
        print(f'Serial error : {error}')

    except Exception as error:
        print(f'Error : {error}')

    finally:
        # release MQTT
        if client is not None:
            client.loop_stop()
            client.disconnect()

        # release Arduino(Serial)
        if arduino is not None and arduino.is_open:
            arduino.close()

        print('Program exit')

if __name__ == '__main__':
    main()