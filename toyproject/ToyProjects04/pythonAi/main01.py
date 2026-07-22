# FastAPI Server 01
from fastapi import FastAPI

app = FastAPI()  # 객체 생성

@app.get('/')
def root():
    return {
        'message': "FastAPI server start!"
    }