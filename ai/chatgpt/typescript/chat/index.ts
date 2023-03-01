import { Context, HttpRequest } from '@azure/functions'
import { Configuration, OpenAIApi } from 'openai' 

export default async function (context: Context, req: HttpRequest): Promise<void> {
    context.log('HTTP trigger function processed a request.');

    const OPENAI_API_KEY = process.env.OPENAI_API_KEY

    if (!OPENAI_API_KEY) {
        context.res.status(406).json({
            error: {
            message: "OpenAI API key not configured, please follow instructions in README.md",
            }
        });
        return;
    }

    const configuration = new Configuration({
        apiKey: process.env.OPENAI_API_KEY,
    });

    const prompt: string | undefined = (req.query.prompt || (req.body && req.body.prompt));
    if (!prompt) {
        context.res.status(500).json({
          error: {
            message: "prompt value is required in the query string or in the request body",
          }
        });
        return;
    }

    try {
      const openaiClient = new OpenAIApi(configuration);
      const completion = await openaiClient.createCompletion({
        model: 'text-davinci-003',
        prompt: generatePrompt(prompt),
        temperature: 0.9,
        max_tokens: 200
      });

      console.log('Completion result: /n' + completion.data.choices[0].text);
      context.res.status(200).json({ result: completion.data.choices[0].text });
    
    } catch(error) {
      // Consider adjusting the error handling logic for your use case
      if (error.response) {
        console.error(error.response.status, error.response.data);
        context.res.status(error.response.status).json(error.response.data);
        return;
      } else {
        console.error('Error with OpenAI API request: ${error.message}');
        context.res.status(500).json({
          error: {
            message: "An error occurred during your request.",
          }
        });
        return;
      }
    }


    function generatePrompt(prompt: string): string {
        const capitalizedPrompt =
          prompt[0].toUpperCase() + prompt.slice(1).toLowerCase();
    
        // prompt template is important to set some context and training up front in addition to user driven input
    
        // Freeform question
        const promptTemplate = `${capitalizedPrompt}`; 
    
        // Chat
        //const promptTemplate = `The following is a conversation with an AI assistant. The assistant is helpful, creative, clever, and very friendly.\n\nHuman: Hello, who are you?\nAI: I am an AI created by OpenAI. How can I help you today?\nHuman: ${capitalizedPrompt}` 
    
        // Classification
        //const promptTemplate = `The following is a list of companies and the categories they fall into:\n\nApple, Facebook, Fedex\n\nApple\nCategory: ` 
    
        // Natural language to Python
        // const promptTemplate = '\"\"\"\n1. Create a list of first names\n2. Create a list of last names\n3. Combine them randomly into a list of 100 full names\n\"\"\"'
    
        return promptTemplate;
      }
};