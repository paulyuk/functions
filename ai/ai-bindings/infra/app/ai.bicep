param name string
param location string = resourceGroup().location
param tags object = {}

param gptDeploymentName string = 'davinci'
param chatGptDeploymentName string = 'chat'
param chatGptModelName string = 'gpt-35-turbo'
param chatGptDeploymentCapacity int = 1
param embeddingsDeploymentName string = 'embeddings'
param embeddingsModelName string = 'text-embedding-ada-002'
param embeddingsDeploymentCapacity int = 60

module openai '../core/ai/cognitiveservices.bicep' = {
  name: 'ai-textanalytics'
  params: {
    name: name
    location: location
    tags: tags
    deployments: [
      {
        name: chatGptDeploymentName
        model: {
          format: 'OpenAI'
          name: chatGptModelName
          version: '0301'
        }
        sku: {
          name: 'Standard'
          capacity: chatGptDeploymentCapacity
        }
      }
      {
        name: embeddingsDeploymentName
        model: {
          format: 'OpenAI'
          name: embeddingsModelName
          version: '2'
        }
        sku: {
          name: 'Standard'
          capacity: embeddingsDeploymentCapacity
        }
      }
    ]
  }
}

output AZURE_OPENAI_SERVICE string = openai.outputs.name
output AZURE_OPENAI_ENDPOINT string = openai.outputs.endpoint
output AZURE_OPENAI_GPT_DEPLOYMENT string = gptDeploymentName
output AZURE_OPENAI_CHATGPT_DEPLOYMENT string = chatGptDeploymentName
output AZURE_OPENAI_EMBEDDINGS_DEPLOYMENT string = embeddingsDeploymentName
