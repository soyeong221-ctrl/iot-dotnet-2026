from pydantic import BaseModel # API로 전달할 기본 모델
from fastapi import FastAPI, UploadFile, File, HTTPException
from pathlib import Path
from email.header import decode_header
from sentence_transformers import SentenceTransformer

import os
import shutil
import re
import uvicorn
# import fitz
import pymupdf

app = FastAPI()

UPLOAD_DIR = Path('uploads')
UPLOAD_DIR.mkdir(exist_ok=True)
# os.makedirs(UPLOAD_DIR, exist_ok=True)

# json과 dictionary로 쉽게 처리하기 위해서
# 속성값을 규칙에 맞게 할당받기 위해서
class QuestionRequest(BaseModel):
    question: str

# 임베딩모델 선언 - 다국어(한국어 포함)용 트랜스포머 모델 필요
embedding_model = SentenceTransformer(
    "sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2"
)

@app.get('/')
def index():
    return {
        'message': 'AI Knowledge Server'
    }

@app.get('/health')
def health():
    return {
        'status': 'OK'
    }

@app.post('/ask')
def ask(request: QuestionRequest):
    return {
        'answer': f'질문을 받음: {request.question}'
    }

## 내부 함수 1. 파일명 공백 제거
def get_safe_filename(filename: str):
    # 경로 제거
    filename = Path(filename).name
    # 파일 앞뒤 공백 제거
    filename = filename.strip()
    # 파일명 사이 공백을 _로 변경
    filename = re.sub(r'\s+', '_', filename)

    return filename

## 내부 함수 2. 같은 파일명 중복 처리
def get_unique_filepath(directory: Path, filename: str):
    file_path = directory / filename

    # 같은 파일이 없으면 그대로 사용
    if not file_path.exists():
        return file_path

    stem = file_path.stem       # 파일 이름만
    suffix = file_path.suffix   # 파일 확장자만
    count = 1

    while True:
        new_filename = f'{stem}_{count}{suffix}' # filename_1.pdf
        new_path = directory / new_filename

        if not new_path.exists():
            return new_path

        count += 1

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

## 내부 함수 5 - Chunk 분리 함수
def split_into_chunk(pages, chunk_size=50, overlap=10):
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

## 내부 함수 6 - Embedding(임베딩) 함수
def create_embedding(chunks):
    texts = [
        chunk['text'] for chunk in chunks
    ]

    embeddings = embedding_model.encode(
        convert_to_numpy=True
    )
    return embeddings

@app.post('/upload')
async def upload(file: UploadFile = File(...)):
#     save_path = os.path.join(UPLOAD_DIR, file.filename)

#     with open(save_path, 'wb') as buffer:
#         shutil.copyfileobj(file.file,buffer)

#     return {
#         'message': '업로드 완료',
#         'filename': file.filename
#     }
    try:
        original_filename = decode_filename(file.filename)        

        print(f'전달된 파일명 : {file.filename}')
        print(f'디코딩 파일명 : {original_filename}')

        # 파일명이 없거나, 확장
        if not original_filename:
            raise HTTPException(status_code=400, detail="파일이 없습니다.")
        
        if not original_filename.lower().endswith('.pdf'):
            raise HTTPException(status_code=400, detail='파일 형식이 잘못되었습니다.')

        # 파일명 변환
        safe_filename = get_safe_filename(original_filename)
        # 중복되지 않는 파일 경로 생성
        save_path = get_unique_filepath(UPLOAD_DIR, safe_filename)

        # 파일 저장
        with save_path.open('wb') as buffer:
            shutil.copyfileobj(file.file, buffer)

        # 파일 저장 이후에 텍스트 추출
        pages = extract_pdf_text(save_path)

        # Chunk 단위로 나누기
        chunks = split_into_chunk(pages)

        # Embedding
        embeddings = create_embedding(chunks)

        print(f'페이지 수: {len(pages)}')
        print(f'Chunk 수: {len(chunks)}')
        print(f'Embedding 수: {len(embeddings)}')

        # if pages:
        #     print('첫 페이지 내용')
        #     print(pages[0]['text'][:100])

        # if chunks:
        #     print('첫 번째 Chunk')
        #     for chunk in chunks:
        #         print(chunk)

        if len(embeddings) > 0:
                 print(f'첫 번째 Embedding: {embeddings[0]}')
                 print(f'벡터 크기: {len(embeddings[0])}')

        return {
            'message': '파일 업로드 완료',
            'original_filename': file.filename,
            'saved_filename': save_path.name,
            'pages': len(pages),
            'chunks': len(chunks)
        }
    except HTTPException:
        raise
    except Exception as e:
        print(f'파일 업로드 오류: {e}')
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == '__main__':
    uvicorn.run('main:app', host='127.0.0.1', port=8000, reload=True)