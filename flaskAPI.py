from flask import Flask, Response, stream_with_context, request, jsonify, render_template, g, redirect, abort, session, url_for
from langchain_community.llms import ollama
from langchain.chains import RetrievalQA
from langchain.memory import ConversationBufferMemory
from langchain.prompts import PromptTemplate
from langchain.callbacks.streaming_stdout import StreamingStdOutCallbackHandler
from langchain.callbacks.manager import CallbackManager
from langchain_community.llms import Ollama
from langchain_community.document_loaders import PyPDFLoader
from langchain_chroma import Chroma
from langchain.text_splitter import RecursiveCharacterTextSplitter, CharacterTextSplitter
from langchain.embeddings.ollama import OllamaEmbeddings
from werkzeug.utils import secure_filename
from chromadb import HttpClient
#from fastapi.middleware.cors import CORSMiddleware?

from fastapi import FastAPI, Depends
from fastapi.security import OAuth2AuthorizationCodeBearer
from starlette.requests import Request

from google.oauth2 import id_token
from google_auth_oauthlib import flow
from pip._vendor import cachecontrol

from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import Flow
import os
import pathlib

import sqlite3
import google.auth.transport.requests
import os
import time
import subprocess
import ollama
import subprocess
import requests
import json
import chromadb
import psutil
import threading
import qrcode
import asyncio
from googleauth import *


#from FlaskWebSite import FlaskWebSiteRoutes

app = Flask(__name__)

fastAPI = FastAPI()

WebSite = Flask('WebSite')


app.secret_key = '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'
os.environ['OAUTHLIB_INSECURE_TRANSPORT'] = '1'

def get_model_name_from_txt():
    with open('Model.txt', 'r') as f:
        return f.read()


# Define the scopes

IsRunning = False

async def add_documents_to_collection(documents):
    # Initialize Ollama embeddings model
    ollama_embed = OllamaEmbeddings(model="mistral:latest")

# Initialize Chroma vector store instance
    vector_store = Chroma(
    collection_name="my_collection",
    embedding_function=ollama_embed,
    persist_directory='db.index/ollama'
    )

# Add documents to the collection
    docs = documents
    vector_store.add_documents(documents=docs)
    return "done"


async def load_pdfs_to_documents():
    return "done"    

@app.before_request
def before_request():
    global g
    if 'directories' not in g:
        g.directories = []
        
    if 'model' not in g:
        g.model = ""
        
    if 'route' not in g:
        g.route = ""
    if 'collection_name' not in g:
        g.collection_name = ""
    if 'directories' not in g:
        g.directories = []
    
   # g.model = ''

ALLOWED_EXTENSIONS = {'txt', 'pdf'}

@app.route('/uploadfile', methods=['GET'])
async def Process_WebSite_Uploaded_File():
    if 'file' not in request.files:
            return 'No file part'
    file = request.files['file']
        # If the user does not select a file, the browser submits an empty file without a filename
    if file.filename == '':
        return 'No selected file'
    if file and allowed_file(file.filename):
        filename = secure_filename(file.filename)
        file.save(os.path.join('files', filename))
    return "File uploaded successfully"

def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS



    

@app.route('/local-embed', methods=['GET'])
async def local_embed():
    Collection_name = request.args.get('param1')
    Model = request.args.get('param2')
    
    
    data = request.get_data()
    directory = data.decode('utf-8')
    if directory != "done":
        g.directories.append(directory)
    if directory == "done":
        await embed_pdf_files_from_directories(g.directories)
    return "Embeddings created"

def local_document_embedings(collection : str , model : str, client : HttpClient):
    loader = PyPDFLoader()
    splitter = RecursiveCharacterTextSplitter(chunk_size=10, chunk_overlap=10, separators=["\n\n", "."])
    vs = Chroma(embedding_function=OllamaEmbeddings(model=model) , client=client, collection_name=collection)
    with open('directories.txt', 'r') as f:
        lines = f.read().splitlines()
        doc_num = 1
        for line in lines:
            yield 'loading document ' + str(doc_num)
            doc = loader.load(r"{line}")
            all_splits = splitter.split_documents(doc)
            vs.add_documents(all_splits)
            doc_num += 1
        yield 'done'
            
            
            
    
@app.get('/login/google')
async def login_google():
    try:
        redirect_uri = url_for('Auth_google', _external=True)
        return await google.authorize_redirect(redirect_uri)
    except Exception as e:
        app.logger.error(f"Error occurred: {e}")
        json.dumps(e)
        return e

@app.route('/authorize/google')
async def Auth_google():
    try:
        token = await google.authorize_access_token()
        userinfo_endpoint = google.server_metadata_url
        response = await google.get(userinfo_endpoint)
        user_info = response.json()
        user_email = user_info['email']
        session['email'] = user_email
        return redirect('/')
    except Exception as e:
        app.logger.error(f"Error occurred: {e}")
        return jsonify(error=str(e)), 500

@app.get('/auth/callback')
async def auth():
    flow.fetch_token(authorization_response=request.url)

    if not session['state'] == request.args['state']:
        return 'State does not match!', 400

    credentials = flow.credentials
    session['credentials'] = credentials_to_dict(credentials)

    return redirect(url_for('drive_files'))


async def embed_pdf_files_from_directories(directories : list[str]):
    try:
# Initialize the loader
        try:
            all_docs = []
            for directory_path in directories:
                loader = PyPDFLoader(directory_path)
                docs = loader.load()
                all_docs.extend(docs)
            text_splitter = RecursiveCharacterTextSplitter(
                    chunk_size=1500,
                    chunk_overlap=200,
                    length_function=len
                    )
            all_splits = text_splitter.split_documents(all_docs)
        except Exception as e:
            app.logger.error(f"Error occurred: {e}")
            return "Error occurred loading"
            # Create and persist the vector store
     #   vectorstore = Chroma.from_documents(
     #               documents=all_splits,
     #               embedding=OllamaEmbeddings(g.model),
     #               persist_directory="data/"
     #           )
     #   vectorstore.persist()
        global g
        with open('col.txt', 'w') as f:
            f.write(g.collection_name)
        vectorestore=Chroma.from_documents(documents=all_splits,embedding=OllamaEmbeddings(model=g.model), persist_directory="data/mistral/latest", collection_name=g.collection_name)
        return "Embeddings created"

    except Exception as e:
        app.logger.error(f"Error occurred: {e}")
        return "Error occurred"



##No Collection Choosen
        
@app.route('/stream', methods=['GET'])
def stream():
    param1 = request.args.get('param1')
    return Response(stream_with_context(generate_AI_response(param1)), mimetype='text/plain')

def generate_AI_response(query :str):
    global g
    stream = ollama.chat(
    model=g.model,
    messages=[{'role': 'user', 'content': query}],
    stream=True,
    )
    for chunk in stream:
        yield chunk['message']['content']
        #print(chunk['message']['content'], end='', flush=True)
        #yield chunk['message']['content']


@app.route('/stop-server', methods=['POST'])
async def stop_server():
    try:
        stop_ollama_server()
        return "Server stopped"
    except Exception as e:
        app.logger.error(f"Error occurred: {e}")
        return "Error occurred"


def stop_ollama_server():
    try:
        for proc in psutil.process_iter(['pid', 'name']):
            if 'ollama_llama_server' in proc.info['name']:
                pid = proc.info['pid']
                print(pid)
                process = psutil.Process(int(pid))
                process.terminate()  # or process.kill() for a forceful kill

    except psutil.NoSuchProcess:
        print(f"No process found with PID {pid}.")
    except psutil.AccessDenied:
        print(f"Access denied to terminate process with PID {pid}.")
    except psutil.TimeoutExpired:
        print(f"Process {pid} did not terminate in time.")
 
@app.route('/model/', methods=['POST'])
async def set_Model():
    data = request.get_data()
    model = data.decode('utf-8')
    command = "ollama pull " + model
    subprocess.run(command, shell=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    return "Model set"
#Causes hold up, the below function
def start_chroma_server(model : str):
    model = model
    directory = os.path.dirname(os.path.abspath(__file__))
    path_add = "/data/" + model.replace(":", "/") + "/"
    path = directory + path_add
    command = "chroma run --path " + path + " --port 8080"
    os.system(command)
    
def model_search_set(query: str):
        with open('Models.txt', 'r') as f:
            models = f.read().splitlines()
            for model in models:
                if model in query:
                    global g
                    g.model = model
                    return model

@app.route('/create-collections', methods=['GET'])
def create_collection_endpoint():
    collection_name = request.args.get('param1')
    model = request.args.get('param2')
    try:
        create_collection(model, collection_name)
    except Exception as e:
        app.logger.error(f"Error occurred: {e}")
        return Response("Error occurred", mimetype='text/plain')

async def create_collection(model : str, collection_name : str):
    loader = PyPDFLoader()
    splitter = RecursiveCharacterTextSplitter(chunk_size=10, chunk_overlap=10, separators=["\n\n", "."])
    yield 'creating ' + collection_name
    client = chromadb.HttpClient(host='localhost', port=8080)
    client.create_collection(collection_name=collection_name, metadata={"model": model})
    yield collection_name + ' created'
    with open('directories.txt', 'a') as f:
        lines = f.read().splitlines()
        for line in lines:
            
    vs = Chroma(embedding_function=OllamaEmbeddings(model=model), client=client, collection_name=collection_name)
    Chroma.from_documents(client=client, collection_name=collection_name, embedding=OllamaEmbeddings(model=model))
    
    
    

@app.route('/list-collections', methods=['GET'])
def get_collection():
    try:
        return Response(stream_with_context(get_collection_names()), mimetype='text/plain')
    except Exception as e:
        app.logger.error(f"Error occurred: {e}")
        return Response("Error occurred", mimetype='text/plain')

def get_collection_names():
    try:
        client = chromadb.HttpClient(host='localhost', port=8080)
        collections = client.list_collections()
        client = chromadb.HttpClient(host='localhost', port=8080)
        collections = client.list_collections()
        for collection in collections:
            coll = client.get_collection(collection.name)
            meta = coll.metadata
            if meta["model"] == "mistral:latest":
                yield collection.name + "\n"
    except Exception as e:
        app.logger.error(f"Error occurred: {e}")
        yield "No collections found"

@app.route('/SetCollection', methods=['POST'])
def SetCollection():
    global g
    g.collection_name = request.get_data().decode('utf-8')
    with open('coll.txt','w') as f:
        f.write(g.collection_name)
    if g.model.find(':') == -1:
        return "Done1"
    else:
        g.route = g.model.replace(':', '/')
        return "Done2"


@app.route('/upload-file', methods=['POST'])
async def create_embeddings():
    if 'file' not in request.files:
        return 'No file part'
    file = request.files['file']
    if file.filename == '':
        return 'No selected file'
    if file:
        filename = secure_filename(file.filename)
        file.save(os.path.join(os.getcwd(), 'files', filename))
        return 'File uploaded successfully'


@app.route('/startwebsite' , methods=['POST'])
def start_website():
    try:
        os.environ['FLASK_APP'] = 'FlaskWebSite.py'

        # Start Flask using subprocess
        subprocess.Popen(['flask', 'run', '--port=8070'])
        with open('running.txt', 'w') as f:
            f.write('True')
        return "Website started"
    except Exception as e:
        app.logger.error(f"Error occurred: {e}")
        return "Error occurred"

@app.route('/stopwebsite' , methods=['POST'])
def stop_website():
    for proc in psutil.process_iter(['pid', 'name', 'connections']):
        for conn in proc.info['connections']:
            print(conn)
            if conn.laddr.port == 8070:
                process = psutil.Process(proc.info['pid'])
                process.terminate()  # or process.kill() for a forceful kill
    #func = request.environ.get('werkzeug.server.shutdown')
    #if func is None:
    #    raise RuntimeError('Not running with the Werkzeug Server')
    #func()
   # command = "ollama run " + model
   # subprocess.run(command, shell=True)
   

   


if __name__ == '__main__':
    app.run(debug=True, port=8060)
    #os.environ['FLASK_APP'] = 'flaskAPI.py'

        # Start Flask using subprocess
    #subprocess.Popen(['flask', 'run', '--port=8060'])