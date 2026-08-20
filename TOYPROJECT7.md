# 토이 프로젝트 7

## AI 문서검색·질의응답 시스템

### 개요

![alt text](image-430.png)

기업 문서를 기반으로 한 AI 지식검색 시스템 개발
- 사내 PDF 문서 등록해 두고, 사용자가 저언어로 질문을 하면 관련 문서를 찾아서 근거와 함께 답변을 해주는 WPF 윈앱 프로그램을 구현

사용 기술
|구분|기술|
|:--:|:---|
|화면|C# WPF|
|서버|Python FastAPI|
|PDF 처리|Python|
|벡터 DB|?|
|AI 모델|Ollama 또는 OpenAI|
|통신|REST API / JSON|
|DB 저장|?|

#### RAG
Retrieval Augmented Generation: 검색(Retrieval) + AI 답변생성(Generation)
- 내가 제공한 문서를 먼저 검색한 뒤 그 내용을 참고해서 답변하는 방식
- 구글 노트북이 대표적인 사이트 https://notebook.google.com/

### 프로젝트 구성
```plaintext
ToyProject07(AIKnowledgeSystem)
│
├─ Client(WPFClient) - 사용자 화면
│
├─ Server(AIServer) - FastAPI + Python Functioins
```

### 클라이언트 구현
#### Visual Studio WPF 프로젝트 생성

WPF 애플리케이션 프로젝트 생성 - .NET 10.0 (LTS) 선택

##### MainWindow.xaml 디자인
![alt text](image-431.png)

##### 파일 선택 구현
![alt text](image-432.png)

##### 서버로 데이터 전송

##### 문서 등록 버튼 추가

![alt text](image-434.png)

##### PDF 전송 기능

![alt text](image-436.png)
- 파일명에 공백이 있으면 업로드 실패
- 동일명의 파일이 올라가면 이전 파일 삭제, 새로 업로드



#### 서버 구현

##### 필요 패키지 설치

- 가상환경에 FastAPI용 패키지 설치(이미 설치됨)
```powershell
> pip install fastapi uvicorn
```

##### FastAPI 서버 구현
- 기본 서버 구현
```python
from fastapi import FastAPI

app = FastAPI()

@app.get('/')
def index():
    return {
        'message': 'AI Knowledge Server'
    }

@app.get('health')
def health():
    return {
        'status': 'OK'
    }
```

- 실행
```powershell
> uvicorn main:app --reload
```

- Swagger UI에서 확인 - [http://localhost:8000/docs](http://localhost:8000/docs)

![alt text](image-433.png)

##### 질문받기 Post API 추가

```python
from pydantic import BaseModel # API로 전달할 기본 모델

# json과 dictionary로 쉽게 처리하기 위해서
# 속성값을 규칙에 맞게 할당받기 위해서
class QuestionRequest(BaseModel):
    question: str

@app.post('/ask')
def ask(request: QuestionRequest):
    return {
        'answer': f'질문을 받음: {request.question}'
    }
```

##### 파일업로드 Post 기능 추가
```powershell
> pip install python-multipart
```

```python
from fastapi import FastAPI, UploadFile, File
import os
import shutil

UPLOAD_DIR = 'uploads'
os.makedirs(UPLOAD_DIR, exist_ok=True)

@app.post('/upload')
async def upload(file: UploadFile = File(...)):
    save_path = os.path.join(UPLOAD_DIR, file.filename)

    with open(save_path, 'wb') as buffer:
        shutil.copyfileobj(file.file, buffer)

    return {
        'message': '업로드 완료',
        'filename': file.filename
    }
```
##### 한글 인코딩 문제 해결

- WPF에서 전달된 한글 인코딩으로 변환된 파일명을 다시 한글파일명으로 디코딩해야 함

```python
from email.header import decode_header

## 내부 함수 3 - UTF8 인코딩으로 전달된 한글문서명 디코딩 함수
def decode_filename(filename: str):
    decoded_parts = decode_header(filename)

    result = ""

    for part, encoding in decoded_parts:
        if isinstance(part, bytes):
            result += part.decode(encoding or "utf-8")
        else:
            result += part

    return result
```

##### PDF 내 텍스트 추출
- 업로드한 PDF를 읽어서 실제 글자를 가져올 수 있는지 확인
- PyMuPDF 패키지

```powershell
> pip install pymupdf
```

- PDF 텍스트 추출 함수

```python
import pymupdf

## 내부 함수 4 - PDF 로드 함수
def extract_pdf_text(pdf_path:Path):
    doc = pymupdf.open(pdf_path)
    pages = []

    for page_number, page in enumerate(doc):
        text = page.get_text()

        pages.append({
            'page': page_number + 1,
            'text': text
        })

    doc.close()
    return pages
```

- 업로드된 PDF 텍스트 추출 확인 - PDF 원본

![alt text](image-437.png)

- 업로드된 PDF 텍스트 추출 확인 - Python에서 추출 확인

![alt text](image-438.png)

##### PDF 내용 Chunk 단위 분리
- RAG는 PDF 전체를 한번에 검색하지 않음. 작은 텍스트 조각으로 나눠서 검색

```python
## 내부 함수 5 - Chunk 분리 함수
def split_into_chunck(pages, chunk_size=50, overlap=10):
    chunks = []
    for page in pages:
        text = page['text']
        start = 0
        chunk_index = 0

        while start < len(text):
            end = start + chunk_size
            chunk_text = text[start:end]

            if chunk_text.strip():
                chunks.append({
                    'page': page['page'],
                    'chunk_index': chunk_index,
                    'text': chunk_text
                })

            start += chunk_size - overlap
            chunk_index += 1

    return chunks
```

- Chunk 분리(길이 50, 오버랩 10) 결과

![alt text](image-439.png)

##### Embedding
- Chunk 문장을 숫자 배열로 바꾸는 작업. 숫자 벡터로 추후 질문과 문서 Chunk 간의 의미가 얼마나 비슷한지 비교
- Sentence Transformers 패키지 설치
    - 텍스트를 고정 길이 벡터로 변환하는 라이브러리
    - 트랜스포머: 자연어 처리시 기존 문제를 해결한 새 매커니즘. BERT, GPT
    - Transformer 신경망 사용해서 문장의 의미를 숫자 벡터로 표현하는 모델

```powershell
> pip install sentence-transformers
```

- 임베딩 함수

```python
from sentence_transformers import SentenceTransformer

# 임베딩모델 선언 - 다국어(한국어 포함)용 트랜스포머 모델 필요
embedding_model = SentenceTransformer(
    "sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2"
)

## 내부 함수 6 - Embedding(임베딩) 함수
def create_embedding(chunks):
    texts = [
        chunk['text'] for chunk in chunks
    ]

    embeddings = embedding_model.encode(
        texts,
        convert_to_numpy=True
    )
    return embeddings
```

- 업로드 후 PDF 변환, Chunk 작업 후 Embedding
- 최초 트랜스포머 모델 다운로드

![alt text](image-440.png)

- Embedding 변환결과

![alt text](image-441.png)