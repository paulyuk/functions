param clusterName string
param location string = resourceGroup().location
param tags object = {}
param skuName string = 'Dev(No SLA)_Standard_E2a_v4'
param skuCapacity int = 1
param databaseName string
param hotCachePeriod string = 'P31D'
param softDeletePeriod string = 'P365D'
param principalId string = ''

resource kustoCluster 'Microsoft.Kusto/clusters@2023-08-15' = {
  name: clusterName
  location: location
  sku: {
    name: skuName
    capacity: skuCapacity
    tier:'Basic'
  }
  zones: ['1','3','2']
  identity: {
    type: 'SystemAssigned'
  }
  tags: tags
  
}

resource kustoDatabase 'Microsoft.Kusto/clusters/databases@2023-08-15' = {
  parent: kustoCluster
  name: databaseName
  location: location
  kind: 'ReadWrite'
  properties: {
    softDeletePeriod: softDeletePeriod
    hotCachePeriod: hotCachePeriod
  }
}


// resource kustoDatabaseAccess 'Microsoft.Kusto/clusters/databases/principalAssignments@2023-08-15' = {
//   parent: kustoDatabase
//   name: 'clusterPrincipalAssignment'
//   properties: {
//     principalId: kustoCluster.identity.principalId
//     principalType: 'App'
//     role: 'Admin'
//     tenantId: subscription().tenantId
//   }
// }

output clusterName string = kustoCluster.name
output principalId string = kustoCluster.identity.principalId
output uri string = kustoCluster.properties.uri
output databaseName string = kustoDatabase.name
output databaseId string = kustoDatabase.id
output connectionString string = '${kustoCluster.properties.uri};Initial Catalog=${kustoDatabase.name};Fed=true'
