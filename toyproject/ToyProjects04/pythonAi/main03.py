# FastAPI Server 03
import uvicorn
from fastapi import FastAPI, HTTPException

app = FastAPI()  # 객체 생성

# json 데이터 생성
products = [
    {
        'id': 1,
        'name': 'Lenovo AI 154',
        'price': 2_500_000  # 2500000
    },
    {
        'id': 2,
        'name': 'MS Bluetooth Keyboard',
        'price': 120_000
    },
    {
        'id': 3,
        'name': 'Lenovo Tap Pro',
        'price': 800_000
    }
]

# 기본 route
@app.get('/')
def root():
    return {
        'message': "FastAPI server start!"
    }

# 전체 데이터
@app.get('/products')
def get_products():
    return products;

# 상세 데이터
@app.get('/products/{product_id}')
def get_product(product_id: int):
    for product in products:
        if product['id'] == product_id:
            return product
        
    raise HTTPException(status_code=404,
                        detail='제품을 찾을 수 없습니다.')

# 쿼리 스트링
@app.get('/search')
def search_products(keyword: str, min_price: int=0):
    # return {
    #     'keyword': keyword,
    #     'min_price': min_price
    # }
    result = []

    for product in products:
        if keyword in product['name'] and min_price <= product['price']:
            result.append(product)

    return result


if __name__ == '__main__':
    uvicorn.run('main03:app', reload=True, port=8080)