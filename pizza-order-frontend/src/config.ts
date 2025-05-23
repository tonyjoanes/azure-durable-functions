const config = {
  apiBaseUrl: process.env.REACT_APP_API_BASE_URL || 'http://localhost:7071',
  apiEndpoints: {
    health: '/api/health',
    order: {
      create: '/api/OrderOrchestrator_HttpStart',
      status: (orderId: string) => `/api/OrderStatus/${orderId}`,
      confirm: (instanceId: string) => `/api/ConfirmOrder?instanceId=${instanceId}`
    }
  },
  corsOptions: {
    credentials: 'include' as RequestCredentials,
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    },
    mode: 'cors' as RequestMode
  }
};

export default config; 