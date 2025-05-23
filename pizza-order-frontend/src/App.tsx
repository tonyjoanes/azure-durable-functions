import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { ChakraProvider, Box, Container, VStack, Heading } from '@chakra-ui/react';
import NewOrder from './components/NewOrder';
import OrderStatus from './components/OrderStatus';

const App: React.FC = () => {
  return (
    <ChakraProvider>
      <Router>
        <Box minH="100vh" bg="gray.50">
          <Container maxW="container.xl" py={8}>
            <VStack spacing={8} align="stretch">
              <Heading as="h1" textAlign="center" color="blue.600">
                Pizza Ordering System
              </Heading>
              
              <Box as="nav" bg="white" p={4} borderRadius="md" shadow="sm">
                <VStack spacing={4} align="stretch">
                  <Link to="/">Place New Order</Link>
                  <Link to="/status">View Order Status</Link>
                </VStack>
              </Box>

              <Box bg="white" p={6} borderRadius="md" shadow="sm">
                <Routes>
                  <Route path="/" element={<NewOrder />} />
                  <Route path="/status" element={<OrderStatus />} />
                </Routes>
              </Box>
            </VStack>
          </Container>
        </Box>
      </Router>
    </ChakraProvider>
  );
};

export default App; 