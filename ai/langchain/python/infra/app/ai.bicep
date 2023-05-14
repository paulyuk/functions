param name string
param location string = resourceGroup().location
param tags object = {}

param deploymentName string = 'chatgpt'

module openai '../core/ai/openai.bicep' = {
  name: 'ai-textanalytics'
  params: {
    name: name
    location: location
    tags: tags
    deployments: [deploymentName]
  }
}

output name string = openai.outputs.name
output url string = openai.outputs.endpoint
output id string = openai.outputs.id
output deploymentName string = deploymentName
