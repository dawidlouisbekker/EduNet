from flask import Flask, render_template, request, jsonify
import ollama

app = Flask(__name__)

# Store questions and answers
qa_history = []

@app.route('/', methods=['GET'])
def index():
    return render_template('base.html')

@app.route('/ask', methods=['POST'])
def ask():
    data = request.json  # Get the JSON data from the request
    question = data.get('question','')

     # Generate an answer from Ollama
    answer = ollama_chat(question)
    
    # Add the question and answer to history
    qa_history.append({'question': question, 'answer': answer})

    # Return the updated chat history
    return jsonify(qa_history=qa_history)

def ollama_chat(user_input):
    #"""Function to interact with the Ollama model."""
    # Using the Ollama API to get a response
    stream = ollama.chat(
        model="mistral:latest",
        messages=[{'role': 'user', 'content': user_input}],
        stream=True,  # Enabling streaming to get the response in chunks
        ) 
        
    

    response = ''
    for chunk in stream:
        response += chunk['message']['content']
        
        
    return response

if __name__ == '__main__':
    app.run(debug=True, port=8060)
