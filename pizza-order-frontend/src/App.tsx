import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { ChakraProvider, Box, Container, Flex, Heading, Text, Button, useColorModeValue, Grid, GridItem, Link as ChakraLink } from '@chakra-ui/react';
import { PhoneIcon, TimeIcon, AtSignIcon } from '@chakra-ui/icons';
import NewOrder from './components/NewOrder';
import OrderStatus from './components/OrderStatus';
import PendingOrders from './components/PendingOrders';

const App: React.FC = () => {
  const bgColor = useColorModeValue('white', 'gray.800');
  const borderColor = useColorModeValue('gray.200', 'gray.700');

  return (
    <ChakraProvider>
      <Router>
        <Box minH="100vh" bg="gray.50" display="flex" flexDirection="column">
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
          <Container maxW="container.xl" py={8} flex="1">
            <Box bg={bgColor} p={6} borderRadius="md" shadow="sm">
              <Routes>
                <Route path="/" element={<NewOrder />} />
                <Route path="/status" element={<OrderStatus />} />
                <Route path="/pending" element={<PendingOrders />} />
              </Routes>
            </Box>
          </Container>

          {/* Footer */}
          <Box as="footer" bg="gray.800" color="white" py={12} mt="auto">
            <Container maxW="container.xl">
              <Grid templateColumns={{ base: "1fr", md: "repeat(2, 1fr)" }} gap={8}>
                {/* Contact Information */}
                <GridItem>
                  <Heading size="md" mb={4} color="red.400">Contact Us</Heading>
                  <Flex direction="column" gap={3}>
                    <Flex align="center" gap={2}>
                      <PhoneIcon boxSize={5} />
                      <Text>(555) 123-4567</Text>
                    </Flex>
                    <Flex align="center" gap={2}>
                      <AtSignIcon boxSize={5} />
                      <Text>123 Pizza Street, Italian Quarter</Text>
                    </Flex>
                    <Flex align="center" gap={2}>
                      <TimeIcon boxSize={5} />
                      <Text>Open Daily: 11AM - 10PM</Text>
                    </Flex>
                  </Flex>
                </GridItem>

                {/* Quick Links */}
                <GridItem>
                  <Heading size="md" mb={4} color="red.400">Quick Links</Heading>
                  <Flex direction="column" gap={2}>
                    <ChakraLink as={Link} to="/" color="gray.300" _hover={{ color: "red.400" }}>
                      Place an Order
                    </ChakraLink>
                    <ChakraLink as={Link} to="/status" color="gray.300" _hover={{ color: "red.400" }}>
                      Track Your Order
                    </ChakraLink>
                    <ChakraLink href="#" color="gray.300" _hover={{ color: "red.400" }}>
                      Menu
                    </ChakraLink>
                    <ChakraLink href="#" color="gray.300" _hover={{ color: "red.400" }}>
                      Careers
                    </ChakraLink>
                  </Flex>
                </GridItem>
              </Grid>

              {/* Copyright */}
              <Box borderTop="1px" borderColor="gray.700" mt={8} pt={8} textAlign="center">
                <Text color="gray.400">
                  © {new Date().getFullYear()} Tony's Pizza. All rights reserved.
                </Text>
              </Box>
            </Container>
          </Box>
        </Box>
      </Router>
    </ChakraProvider>
  );
};

export default App; 