param name string
param location string = resourceGroup().location
param tags object = {}

param gptDeploymentName string = 'davinci'
param gptModelName string = 'text-davinci-003'
param gptDeploymentCapacity int = 29
param chatGptDeploymentName string = 'chat'
param chatGptModelName string = 'gpt-35-turbo'
param chatGptDeploymentCapacity int = 29
param embeddingsDeploymentName string = 'embeddings'
param embeddingsModelName string = 'text-embedding-ada-002'
param embeddingsDeploymentCapacity int = 120

module openai '../core/ai/openai.bicep' = {
  name: 'ai-textanalytics'
  params: {
    name: name
    location: location
    tags: tags
    deployments: [
      {
        name: gptDeploymentName
        model: {
          format: 'OpenAI'
          name: gptModelName
          version: '1'
        }
        capacity: gptDeploymentCapacity
      }
      {
        name: chatGptDeploymentName
        model: {
          format: 'OpenAI'
          name: chatGptModelName
          version: '0301'
        }
        capacity: chatGptDeploymentCapacity
      }
      {
        name: embeddingsDeploymentName
        model: {
          format: 'OpenAI'
          name: embeddingsModelName
          version: '2'
        }
        capacity: embeddingsDeploymentCapacity
      }
    ]
  }
}

output AZURE_OPENAI_SERVICE string = openai.outputs.name
output AZURE_OPENAI_ENDPOINT string = openai.outputs.endpoint
output AZURE_OPENAI_GPT_DEPLOYMENT string = gptDeploymentName
output AZURE_OPENAI_CHATGPT_DEPLOYMENT string = chatGptDeploymentName
output AZURE_OPENAI_EMBEDDINGS_DEPLOYMENT string = embeddingsDeploymentName
