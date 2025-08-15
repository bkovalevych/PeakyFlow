targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
param principalId string

@description('')
param principalType string


resource storageAccount_WI85C0k41 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: toLower(take('peakystorage${uniqueString(resourceGroup().id)}', 24))
  location: location
  tags: {
    'aspire-resource-name': 'peakystorage'
  }
  sku: {
    name: 'Standard_GRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    networkAcls: {
      defaultAction: 'Allow'
    }
    minimumTlsVersion: 'TLS1_2'
    allowSharedKeyAccess: false
  }
}

resource blobService_ji4zkLtfi 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  parent: storageAccount_WI85C0k41
  name: 'default'
  properties: {
  }
}

resource roleAssignment_DPQiGVPHy 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: storageAccount_WI85C0k41
  name: guid(storageAccount_WI85C0k41.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'))
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
    principalId: principalId
    principalType: principalType
  }
}

resource roleAssignment_YWObNvASA 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: storageAccount_WI85C0k41
  name: guid(storageAccount_WI85C0k41.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3'))
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3')
    principalId: principalId
    principalType: principalType
  }
}

resource roleAssignment_K4GlHxLig 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: storageAccount_WI85C0k41
  name: guid(storageAccount_WI85C0k41.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '974c5e8b-45b9-4653-ba55-5f855dd0fb88'))
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '974c5e8b-45b9-4653-ba55-5f855dd0fb88')
    principalId: principalId
    principalType: principalType
  }
}

output blobEndpoint string = storageAccount_WI85C0k41.properties.primaryEndpoints.blob
output queueEndpoint string = storageAccount_WI85C0k41.properties.primaryEndpoints.queue
output tableEndpoint string = storageAccount_WI85C0k41.properties.primaryEndpoints.table
