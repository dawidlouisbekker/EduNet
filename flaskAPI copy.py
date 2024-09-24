from flask import Flask, Response, stream_with_context, request, jsonify, render_template


app = Flask(__name__)

@app.route('/')
async def index():
    return render_template('base.html')
        
    

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=8060)