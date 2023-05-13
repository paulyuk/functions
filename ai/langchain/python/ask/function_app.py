import azure.functions as func
import logging
import os
import openai
from langchain.llms import OpenAI
from langchain.llms.openai import AzureOpenAI

app = func.FunctionApp()

@app.function_name(name='chat')
@app.route(route='chat')
def main(req):

    USE_LANGCHAIN = os.getenv("ENV_VAR", 'True').lower() in ('true', '1', 't')
    AZURE_OPENAI_KEY = os.environ.get("AZURE_OPENAI_KEY")
    AZURE_OPENAI_ENDPOINT = os.environ.get("AZURE_OPENAI_ENDPOINT")
    if 'AZURE_OPENAI_KEY' not in os.environ:
        raise RuntimeError("No 'AZURE_OPENAI_KEY' env var set.  Please see Readme.")

    # configure azure open ai for langchain

    # Replace these with your own values, either in environment variables or directly here
    AZURE_OPENAI_SERVICE = os.environ.get("AZURE_OPENAI_SERVICE") or "myopenai"
    AZURE_OPENAI_GPT_DEPLOYMENT = os.environ.get("AZURE_OPENAI_GPT_DEPLOYMENT") or "davinci"
    AZURE_OPENAI_CHATGPT_DEPLOYMENT = os.environ.get("AZURE_OPENAI_CHATGPT_DEPLOYMENT") or "chat" #GPT turbo

    if bool(USE_LANGCHAIN):
        logging.info('Using Langchain')

        openai.api_key = os.getenv('AZURE_OPENAI_KEY')
        openai.api_base = os.getenv("AZURE_OPENAI_ENDPOINT") # your endpoint should look like the following https://YOUR_RESOURCE_NAME.openai.azure.com/
        openai.api_type = 'azure'
        openai.api_version = '2022-12-01' # this may change in the future

        # **LangChain
        llm = AzureOpenAI(deployment_name=AZURE_OPENAI_CHATGPT_DEPLOYMENT, temperature=0.3, openai_api_key=AZURE_OPENAI_KEY)
        text = "What would be a good company name for a company that makes colorful socks?"

        return llm(text)

    else:

        logging.info('Using CHATGPT LLM directly')

        prompt = req.params.get('prompt') 
        if not prompt: 
            try: 
                req_body = req.get_json() 
            except ValueError: 
                raise RuntimeError("prompt data must be set in POST.") 
            else: 
                prompt = req_body.get('prompt') 
                if not prompt:
                    raise RuntimeError("prompt data must be set in POST.")

        completion = openai.Completion.create(
            engine=AZURE_OPENAI_CHATGPT_DEPLOYMENT,
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
