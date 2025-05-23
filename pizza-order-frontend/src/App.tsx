import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { ChakraProvider, Box, Container, Flex, Heading, Text, Button, useColorModeValue } from '@chakra-ui/react';
import NewOrder from './components/NewOrder';
import OrderStatus from './components/OrderStatus';
import PendingOrders from './components/PendingOrders';

const App: React.FC = () => {
  const bgColor = useColorModeValue('white', 'gray.800');
  const borderColor = useColorModeValue('gray.200', 'gray.700');

  return (
    <ChakraProvider>
      <Router>
        <Box minH="100vh" bg="gray.50">
          {/* Header */}
          <Box as="header" bg="red.600" color="white" py={4} shadow="md">
            <Container maxW="container.xl">
              <Flex justify="space-between" align="center">
                <Heading as="h1" size="xl" fontFamily="'Poppins', sans-serif">
                  Tony's Pizza
                </Heading>
                <Text fontSize="sm" opacity={0.9}>Authentic Italian Taste</Text>
              </Flex>
            </Container>
          </Box>

          {/* Navigation */}
          <Box as="nav" bg={bgColor} borderBottom="1px" borderColor={borderColor} shadow="sm">
            <Container maxW="container.xl">
              <Flex py={4} gap={6}>
                <Button
                  as={Link}
                  to="/"
                  variant="ghost"
                  colorScheme="red"
                  _hover={{ bg: 'red.50' }}
                >
                  Place Order
                </Button>
                <Button
                  as={Link}
                  to="/status"
                  variant="ghost"
                  colorScheme="red"
                  _hover={{ bg: 'red.50' }}
                >
                  Track Order
                </Button>
                <Button
                  as={Link}
                  to="/pending"
                  variant="ghost"
                  colorScheme="red"
                  _hover={{ bg: 'red.50' }}
                >
                  Pending Orders
                </Button>
              </Flex>
            </Container>
          </Box>

          {/* Main Content */}
          <Container maxW="container.xl" py={8}>
            <Box bg={bgColor} p={6} borderRadius="md" shadow="sm">
              <Routes>
                <Route path="/" element={<NewOrder />} />
                <Route path="/status" element={<OrderStatus />} />
                <Route path="/pending" element={<PendingOrders />} />
              </Routes>
            </Box>
          </Container>
        </Box>
      </Router>
    </ChakraProvider>
  );
};

export default App; 