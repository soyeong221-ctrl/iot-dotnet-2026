## 파이썬 MQTT Publish 
# paho-mqtt 라이브러리 설치
# pip install paho-mqtt
import paho.mqtt.client as mqtt
import datetime as dt
import time
import uuid
import json
from collections import OrderedDict
import random

PUB_ID = 'IOT52' # 본인 아이피 마지막주소
BROKER = '210.119.12.52' # 본인 아이피
PORT = 1883
TOPIC = 'smartfactory/52/topic'  # publish/subscribe에서 사용할 토픽(핵심)
# COLORS = ['RED', 'ORANGE', 'YELLOW', 'GREEN', 'BLUE', 'NAVY', 'PURPLE']
COUNT = 0

# 연결 콜백
def on_connect(client, userdata, flags, reason_code, properties=None):
    print(f'Connected with reason code : {reason_code}')    

# 퍼블리시 완료후 발생 콜백
def on_publish(client, userdata, mid):
    print(f'Message published mid : {mid}')

try:
    client= mqtt.Client(client_id=PUB_ID, protocol=mqtt.MQTTv5, userdata=None)
    client.username_pw_set(username='root', password='mqtt123456')
    
    client.on_connect = on_connect
    client.on_publish = on_publish

    client.connect(BROKER, PORT)
    client.loop_start() 

    while True:
        ## OrderedDict -> json 타입으로 변경
        pub_data = json.dumps(f'TEST!! {COUNT}', ensure_ascii=False, indent='\t')
        COUNT += 1

        ## payload에 json 데이터를 할당        
        client.publish(TOPIC, payload=pub_data, qos=1)
        time.sleep(1) 

except Exception as ex:
    print(f'Error raised : {ex}')
except KeyboardInterrupt:
    print('MQTT 전송중단')
    client.loop_stop()
    client.disconnect()