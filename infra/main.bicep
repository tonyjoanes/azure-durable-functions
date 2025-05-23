@description('The name of the Function App')
param functionAppName string

@description('The location for all resources')
param location string = resourceGroup().location

@description('The runtime stack of the Function App')
param functionAppRuntime string = 'dotnet'

@description('The version of the Function App runtime')
param functionAppRuntimeVersion string = '6.0'

@description('The storage account SKU')
param storageAccountSku string = 'Standard_LRS'

@description('The Function App service plan SKU')
param functionAppSku string = 'Y1'

@description('The Function App service plan size')
param functionAppSize string = 'Y1'

// Storage account for Durable Functions
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: toLower('st${functionAppName}')
  location: location
  sku: {
    name: storageAccountSku
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
  }
}

// Application Insights for monitoring
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'ai-${functionAppName}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// Function App service plan
resource functionAppPlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: 'plan-${functionAppName}'
  location: location
  sku: {
    name: functionAppSku
    size: functionAppSize
  }
  kind: 'functionapp'
  properties: {
    reserved: true
  }
}

// Function App
resource functionApp 'Microsoft.Web/sites@2021-02-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: functionAppPlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionAppName)
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: functionAppRuntime
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME_VERSION'
          value: functionAppRuntimeVersion
        }
      ]
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Output the Function App hostname
output functionAppHostname string = functionApp.properties.defaultHostName

// Output the Function App name
output functionAppName string = functionApp.name

// Output the storage account name
output storageAccountName string = storageAccount.name

// Output the Application Insights instrumentation key
output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey 