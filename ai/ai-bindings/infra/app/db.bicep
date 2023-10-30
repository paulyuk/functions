param name string
param location string = resourceGroup().location
param tags object = {}

param databaseName string = ''
param principalId string = ''

// Because databaseName is optional in main.bicep, we make sure the database name is set here.
var defaultDatabaseName = 'vectorsearch'
var actualDatabaseName = !empty(databaseName) ? databaseName : defaultDatabaseName

module dataExplorerDatabase '../core/database/azuredataexplorer/azuredataexplorer.bicep' = {
  name: 'dataExplorerDatabase'
  params: {
    clusterName: name
    location: location
    tags: tags
    databaseName: actualDatabaseName
    principalId: principalId
  }
}

output clusterName string = dataExplorerDatabase.outputs.clusterName
output databaseName string = dataExplorerDatabase.outputs.databaseName
output uri string = dataExplorerDatabase.outputs.uri
output connectionString string = dataExplorerDatabase.outputs.connectionString
output principalId string = dataExplorerDatabase.outputs.principalId

