const config = {
  apiBaseUrl: 'http://localhost:7071',  // Direct to Azure Functions
  apiEndpoints: {
    health: '/api/health',
    order: {
      create: '/api/OrderOrchestrator_HttpStart',
      status: (orderId: string) => `/api/OrderStatus/${orderId}`,
      confirm: (instanceId: string) => `/api/ConfirmOrder?instanceId=${instanceId}`
    }
  }
};

export default config; 