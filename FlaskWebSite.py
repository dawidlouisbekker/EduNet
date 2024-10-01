from flask import Blueprint, render_template
from flask import Flask, render_template, request, jsonify, Response, stream_with_context
import ollama
from flaskAPI import generate_AI_response

        

FlaskWebSiteRoutes = Flask( __name__)



# Store questions and answers
qa_history = []

@FlaskWebSiteRoutes.route('/', methods=['GET'])
def index():
    return render_template('base.html')

@FlaskWebSiteRoutes.route('/ask', methods=['GET'])
def ask_get():
    data = request.json  # Get the JSON data from the request
    question = data.get('question','')

     # Generate an answer from Ollama
    answer = ollama_chat(question)
    
    # Add the question and answer to history
    qa_history.append({'question': question, 'answer': answer})

    # Return the updated chat history
    return jsonify(qa_history=qa_history)

@FlaskWebSiteRoutes.route('/stream', methods=['GET'])
def ask():
    param1 = request.args.get('param1')
    with open('query.txt', 'w') as f:
        f.write(param1)
        print(param1)
    return Response(stream_with_context(generate_AI_response(param1)), mimetype='text/plain')

def ollama_chat(user_input):
    #"""Function to interact with the Ollama model."""
    # Using the Ollama API to get a response
    stream = ollama.chat(
        model="mistral:latest",
        messages=[{'role': 'user', 'content': user_input}],
        stream=True,  # Enabling streaming to get the response in chunks
        )     
    for chunk in stream:
        print(chunk['message']['content'])
        yield chunk['message']['content'] #.encode('utf-8')
        
        
        


@FlaskWebSiteRoutes.route('/PrivacyPolicy')
def PrivacyPolicy():
    return render_template('PrivacyPolicy.html')

@FlaskWebSiteRoutes.route('/TermsOfService')
def TermsOfService():
    return render_template('TermsOfService.html')

#if __name__ == '__main__':
#    FlaskWebSiteRoutes.run(debug=True, port=8070, host='0.0.0.0')


