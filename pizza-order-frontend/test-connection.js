const http = require('http');

console.log('Testing direct connection to Azure Functions...');

// Test 1: Simple HTTP request
const options = {
  hostname: 'localhost',
  port: 7071,
  path: '/api/health',
  method: 'GET',
  headers: {
    'Accept': 'application/json',
    'User-Agent': 'Node.js Test'
  }
};

console.log('\n--- Test 1: Direct HTTP request ---');
const req = http.request(options, (res) => {
  console.log(`Status: ${res.statusCode}`);
  console.log(`Headers:`, res.headers);
  
  let data = '';
  res.on('data', (chunk) => {
    data += chunk;
  });
  
  res.on('end', () => {
    console.log('Response:', data);
    
    // Test 2: Test with different host header
    console.log('\n--- Test 2: Request with proxy-like headers ---');
    testWithProxyHeaders();
  });
});

req.on('error', (e) => {
  console.error(`Request error: ${e.message}`);
});

req.setTimeout(5000, () => {
  console.error('Request timed out after 5 seconds');
  req.destroy();
});

req.end();

function testWithProxyHeaders() {
  const proxyOptions = {
    hostname: 'localhost',
    port: 7071,
    path: '/api/health',
    method: 'GET',
    headers: {
      'Accept': 'application/json',
      'Host': 'localhost:3001',  // Simulate proxy host header
      'X-Forwarded-For': '127.0.0.1',
      'X-Forwarded-Proto': 'http',
      'User-Agent': 'Proxy Test'
    }
  };

  const proxyReq = http.request(proxyOptions, (res) => {
    console.log(`Status: ${res.statusCode}`);
    console.log(`Headers:`, res.headers);
    
    let data = '';
    res.on('data', (chunk) => {
      data += chunk;
    });
    
    res.on('end', () => {
      console.log('Response:', data);
      console.log('\n--- Tests completed ---');
    });
  });

  proxyReq.on('error', (e) => {
    console.error(`Proxy request error: ${e.message}`);
  });

  proxyReq.setTimeout(5000, () => {
    console.error('Proxy request timed out after 5 seconds');
    proxyReq.destroy();
  });

  proxyReq.end();
} 