import azure.functions as func
import logging
import os

import openai
openai.api_key = os.getenv('OPENAI_API_KEY')

app = func.FunctionApp()

@app.function_name(name='chat')
@app.route(route='chat')
def main(req):
    if not open.api_key:
        func.HttpResponse("OpenAI API key not configured, please follow instructions in README.md", 406)
    prompt = req.params.get('prompt') 
    if not prompt: 
        try: 
            req_body = req.get_json() 
        except ValueError: 
            pass 
        else: 
            prompt = req_body.get('prompt') 

    completion = openai.Completion.create(
        model='text-davinci-003',
        prompt=generate_prompt(prompt),
        temperature=0.9,
        max_tokens=200
    )
    return completion.choices[0].text


def generate_prompt(prompt):
    capitalized_prompt = prompt.capitalize()

    # prompt template is important to set some context and training up front in addition to user driven input

    # Freeform question
    return f'{capitalized_prompt}'

    # Chat
    #return f'The following is a conversation with an AI assistant. The assistant is helpful, creative, clever, and very friendly.\n\nHuman: Hello, who are you?\nAI: I am an AI created by OpenAI. How can I help you today?\nHuman: {capitalized_prompt}' 

    # Classification
    #return 'The following is a list of companies and the categories they fall into:\n\nApple, Facebook, Fedex\n\nApple\nCategory: ' 

    # Natural language to Python
    #return '\"\"\"\n1. Create a list of first names\n2. Create a list of last names\n3. Combine them randomly into a list of 100 full names\n\"\"\"'
