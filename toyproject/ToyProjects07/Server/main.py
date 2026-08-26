from pydantic import BaseModel  # API로 전달할 기본 모델
from fastapi import FastAPI, UploadFile, File, HTTPException
from pathlib import Path
from email.header import decode_header
from sentence_transformers import SentenceTransformer

import os
import shutil
import re
import uvicorn
import pymupdf
import chromadb
import ollama
import time

# from openai import OpenAI
# openai_client = OpenAI()

app = FastAPI()

UPLOAD_DIR = Path('uploads')
UPLOAD_DIR.mkdir(exist_ok=True)

# os.makedirs(UPLOAD_DIR, exist_ok=True)


# ChromaDB 설정
chroma_client = chromadb.PersistentClient(
    path='chroma_db'
)

collection = chroma_client.get_or_create_collection(
    name='documents'
)


# json과 dictionary로 쉽게 처리하기 위해서
# 속성값을 규칙에 맞게 할당받기 위해서
class QuestionRequest(BaseModel):
    question: str


# 임베딩모델 선언 - 다국어(한국어 포함)용 트랜스포머 모델 필요
embedding_model = SentenceTransformer(
    "sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2"
)


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
        new_filename = f'{stem}_{count}{suffix}'  # filename_1.pdf
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
def extract_pdf_text(pdf_path: Path):
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
        texts,
        convert_to_numpy=True
    )

    return embeddings


## 내부 함수 7 - Chunk 저장 함수
def save_chunk_to_chroma(chunks, embeddings, filename):
    ids = []
    documents = []
    metadatas = []
    embedding_list = []

    for i, chunk in enumerate(chunks):
        ids.append(
            f'{filename}_{chunk["page"]}_{chunk["chunk_index"]}'
        )

        documents.append(chunk['text'])

        metadatas.append({
            'filename': filename,
            'page': chunk['page'],
            'chunk_index': chunk['chunk_index']
        })

        embedding_list.append(
            embeddings[i].tolist()
        )

    collection.add(
        ids=ids,
        documents=documents,
        metadatas=metadatas,
        embeddings=embedding_list
    )


## 내부 함수 8 - Chunk 검색 함수
### top_k: 관련있는 값을 몇 개까지 가져올 것인지
def search_documents(question: str, top_k=3):

    # 질문을 Embedding으로 변환
    question_embedding = embedding_model.encode(
        question,
        convert_to_numpy=True
    )

    # ChromaDB 검색
    results = collection.query(
        query_embeddings=[
            question_embedding.tolist()
        ],
        n_results=top_k
    )

    return results


## 내부 함수 9 - Ollama 프롬프트생성 함수
def generate_answer(question: str, documents: list):

    context = "\n\n".join(documents)

    prompt = f"""
다음 문서 내용을 참조해서 질문에 답변하세요.
문서에 없는 내용은 추측하지 말고 모른다고 답변하세요.

답변은 핵심 내용만 2~3문장으로 간결하게 작성하세요.

[문서]

{context}

[질문]

{question}

[답변]
    """

    response = ollama.chat(
        model='qwen3.5:2b',

        messages=[
            {
                'role': 'user',
                'content': prompt
            }
        ],

        think=False,

        options={
            'num_predict': 100,
            'temperature': 0.2
        },

        keep_alive='10m'
    )

    return response['message']['content']

### HTTP 메서드 함수

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

    # 전체 처리 시간 측정
    total_start = time.time()

    print("\n===== 질문 처리 시작 =====")
    print(f"질문: {request.question}")

    # DB에서 관련어 검색
    search_start = time.time()

    results = search_documents(
        request.question,
        top_k=3
    )

    search_time = time.time() - search_start

    print(f"1. 벡터 검색 시간: {search_time:.2f}초")

    documents = results['documents'][0]
    metadatas = results['metadatas'][0]
    distances = results['distances'][0]

    print(f"2. 검색된 Chunk 수: {len(documents)}")
    print(f"3. LLM에 전달하는 글자 수: {sum(len(x) for x in documents)}")

    # 검색된 결과를 LLM에 전달
    llm_start = time.time()

    answer = generate_answer(
        request.question,
        documents
    )

    llm_time = time.time() - llm_start

    print(f"4. Ollama 답변 생성 시간: {llm_time:.2f}초")

    total_time = time.time() - total_start

    print(f"5. 전체 처리 시간: {total_time:.2f}초")
    print("===== 질문 처리 완료 =====\n")

    # 결과 반환
    return {
        'question': request.question,
        'answer': answer,
        'sources': metadatas,
        'distances': distances
    }


@app.post('/upload')
async def upload(file: UploadFile = File(...)):

    # save_path = os.path.join(UPLOAD_DIR, file.filename)
    # with open(save_path, 'wb') as buffer:
    #     shutil.copyfileobj(file.file,buffer)
    # return {
    #     'message': '업로드 완료',
    #     'filename': file.filename
    # }

    try:
        original_filename = decode_filename(file.filename)

        print(f'전달된 파일명 : {file.filename}')
        print(f'디코딩 파일명 : {original_filename}')

        # 파일명이 없거나 확장자가 PDF가 아닌 경우
        if not original_filename:
            raise HTTPException(
                status_code=400,
                detail="파일이 없습니다."
            )

        if not original_filename.lower().endswith('.pdf'):
            raise HTTPException(
                status_code=400,
                detail='파일 형식이 잘못되었습니다.'
            )

        # 파일명 변환
        safe_filename = get_safe_filename(
            original_filename
        )

        # 중복되지 않는 파일 경로 생성
        save_path = get_unique_filepath(
            UPLOAD_DIR,
            safe_filename
        )

        # 파일 저장
        with save_path.open('wb') as buffer:
            shutil.copyfileobj(
                file.file,
                buffer
            )

        # 파일 저장 이후에 텍스트 추출
        pages = extract_pdf_text(
            save_path
        )

        # Chunk 단위로 나누기
        chunks = split_into_chunk(
            pages
        )

        # Embedding
        embeddings = create_embedding(
            chunks
        )

        # ChromaDB 저장
        save_chunk_to_chroma(
            chunks,
            embeddings,
            save_path.name
        )

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
            print(
                f'첫 번째 Embedding: {embeddings[0]}'
            )
            print(
                f'벡터 크기: {len(embeddings[0])}'
            )

        return {
            'message': '파일 업로드 완료',
            'original_filename': file.filename,
            'saved_filename': save_path.name,
            'pages': len(pages),
            'chunks': len(chunks),
            'embeddings': len(embeddings)
        }

    except HTTPException:
        raise

    except Exception as e:
        print(f'파일 업로드 오류: {e}')

        raise HTTPException(
            status_code=500,
            detail=str(e)
        )


if __name__ == '__main__':
    uvicorn.run(
        'main:app',
        host='127.0.0.1',
        port=8000,
        reload=True
    )