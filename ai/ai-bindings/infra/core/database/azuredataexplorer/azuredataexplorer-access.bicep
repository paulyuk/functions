param clusterName string
param databaseName string
param principalId string

resource functionToKustoDatabaseAccess 'Microsoft.Kusto/clusters/databases/principalAssignments@2023-08-15' = {
  parent: existingVectorDatabase
  name: 'functionPrincipalAssignment'
  properties: {
    principalId: principalId
    principalType: 'App'
    role: 'Admin'
    tenantId: subscription().tenantId
  }
}

resource existingCluster 'Microsoft.Kusto/clusters@2023-08-15' existing = {
  name: clusterName
}

resource existingVectorDatabase 'Microsoft.Kusto/clusters/databases@2023-08-15' existing = {
  name: databaseName
  parent: existingCluster
}
