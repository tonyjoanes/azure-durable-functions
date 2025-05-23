const express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');
const cors = require('cors');

const app = express();
const PORT = 3001;

// Enable CORS for all requests
app.use(cors());

// Proxy middleware
app.use('/api', createProxyMiddleware({
  target: 'http://localhost:7071',
  changeOrigin: true,
  onProxyReq: (proxyReq, req, res) => {
    console.log(`[PROXY] ${req.method} ${req.url} -> http://localhost:7071${req.url}`);
  },
  onProxyRes: (proxyRes, req, res) => {
    console.log(`[PROXY] Response: ${proxyRes.statusCode} for ${req.url}`);
  },
  onError: (err, req, res) => {
    console.error(`[PROXY] Error: ${err.message} for ${req.url}`);
    res.status(500).send('Proxy Error');
  }
}));

// Health check for the proxy itself
app.get('/proxy-health', (req, res) => {
  res.json({ status: 'Proxy server running', port: PORT });
});

app.listen(PORT, () => {
  console.log(`Proxy server running on http://localhost:${PORT}`);
  console.log(`API requests will be forwarded to http://localhost:7071`);
}); 