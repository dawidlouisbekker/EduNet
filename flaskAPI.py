from flask import Flask, Response, stream_with_context, request, jsonify
from langchain_community.llms import ollama
from langchain.chains import RetrievalQA
from langchain.memory import ConversationBufferMemory
from langchain.prompts import PromptTemplate
from langchain.callbacks.streaming_stdout import StreamingStdOutCallbackHandler
from langchain.callbacks.manager import CallbackManager
from langchain_community.llms import Ollama
from langchain_community.document_loaders import PyPDFLoader
from langchain.vectorstores import Chroma
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain.embeddings.ollama import OllamaEmbeddings
import time
import subprocess
import ollama

def get_model() -> str:
    with open('Model.txt', 'r') as f:
        model = f.read()
    return model

def embed_pdf_files() -> bool:
    try:
# Initialize the loader
        all_docs = []
        directory_paths = []
        with open ('directories.txt', 'r') as f:
            lines = f.read().splitlines()
            for line in lines:
                    loader = PyPDFLoader(line)
                    docs = loader.load_and_split()
                    all_docs.extend(docs)
                    text_splitter = RecursiveCharacterTextSplitter(
                    chunk_size=1500,
                    chunk_overlap=200,
                    length_function=len
                    )
                    all_splits = text_splitter.split_documents(docs)
                    with open('colecName.txt', 'r') as f:
                        directory = f.read()
            # Create and persist the vector store
                    vectorstore = Chroma.from_documents(
                    documents=all_splits,
                    embedding=OllamaEmbeddings(model=get_model()),
                    persist_directory=directory
                    )
                    vectorstore.persist()

        return True

    except Exception as e:
        return False

app = Flask(__name__)

def generate_data():
    for i in range(40):
        yield f"Received value: {i}, frame {i}\n"
        time.sleep(0.1)

def generate_AI_response(query :str):
    stream = ollama.chat(
    model=get_model(),
    messages=[{'role': 'user', 'content': query}],
    stream=True,
    )

    for chunk in stream:
        print(chunk['message']['content'], end='', flush=True)
        yield chunk['message']['content']
        
@app.route('/stream', methods=['GET'])
def stream():
    param1 = request.args.get('param1')
    return Response(stream_with_context(generate_AI_response(param1)), mimetype='text/plain')


@app.route('/collection-name', methods=['POST'])
async def query():
    data = request.get_data()
    with open('colecName.txt', 'a') as f:
        f.write(data.decode('utf-8') + "\n")
    return jsonify({'message': 'Query created'})



@app.route('/local-embed', methods=['POST'])
async def embed():
    data = request.get_data()
    directory = data.decode('utf-8')
    with open('directories.txt', 'a') as f:
        f.write(directory + "\n")
    if directory == "done":
        with open('directories.txt', 'w') as f:
            f.write("")
        return jsonify({'message': 'Embeddings created'})


@app.route('/model', methods=['POST'])
async def set_Model():
    data = request.get_data()
    model = data.decode('utf-8')
    with open('Model.txt', 'r') as f:
        f.write(model + "\n")
    command = "ollama run " + model
    subprocess.run(command, shell=True)

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=8060)