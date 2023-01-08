param name string
param location string = resourceGroup().location
param tags object = {}

module aiLanguageService '../core/cognitive/ai-textanalytics.bicep' = {
  name: 'ai-textanalytics'
  params: {
    aiResourceName: name
    location: location
    tags: tags
  }
}

output url string = aiLanguageService.outputs.url
