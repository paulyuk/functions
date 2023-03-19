import azure.functions as func
import logging

app = func.FunctionApp(http_auth_level=func.AuthLevel.ANONYMOUS)

# Learn more at aka.ms/pythonprogrammingmodel
@app.function_name(name="http")
@app.route(route="http")
def test_function(req: func.HttpRequest) -> func.HttpResponse:
     logging.info('Python HTTP trigger function processed a request.')

     name = req.params.get('name')
     if not name:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            name = req_body.get('name')

     if name:
        hello = f"Hello, {name}. This HTTP triggered function executed successfully."
        logging.info(hello)
        return func.HttpResponse(hello)
     else:
        hello = "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        logging.info(hello)
        return func.HttpResponse(hello, status_code=200)
