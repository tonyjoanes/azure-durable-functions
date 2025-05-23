import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  FormControl,
  FormLabel,
  Input,
  Select,
  VStack,
  useToast,
  Text,
  Alert,
  AlertIcon,
  AlertTitle,
  AlertDescription,
  Button as ChakraButton,
} from '@chakra-ui/react';
import config from '../config';

interface PizzaOrder {
  size: string;
  toppings: string[];
  address: string;
  phone: string;
}

const MAX_RETRIES = 3;
const RETRY_DELAY = 2000; // 2 seconds

const NewOrder: React.FC = () => {
  const [order, setOrder] = useState<PizzaOrder>({
    size: '',
    toppings: [],
    address: '',
    phone: '',
  });
  const [loading, setLoading] = useState(false);
  const [backendAvailable, setBackendAvailable] = useState<boolean | null>(null);
  const [retryCount, setRetryCount] = useState(0);
  const toast = useToast();

  const checkBackend = async () => {
    try {
      const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.health}`, {
        method: 'GET',
        headers: {
          'Accept': 'application/json'
        }
      });
      setBackendAvailable(response.ok);
      if (response.ok) {
        setRetryCount(0); // Reset retry count on success
      }
    } catch (error) {
      console.error('Backend health check failed:', error);
      setBackendAvailable(false);
      
      // Implement retry logic
      if (retryCount < MAX_RETRIES) {
        setTimeout(() => {
          setRetryCount(prev => prev + 1);
        }, RETRY_DELAY);
      }
    }
  };

  // Check backend availability on component mount and when retry count changes
  useEffect(() => {
    checkBackend();
  }, [retryCount]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.order.create}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        body: JSON.stringify(order),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      
      toast({
        title: 'Order Placed!',
        description: `Your order has been submitted. Order ID: ${data.id}`,
        status: 'success',
        duration: 5000,
        isClosable: true,
      });

      // Reset form
      setOrder({
        size: '',
        toppings: [],
        address: '',
        phone: '',
      });
    } catch (error) {
      console.error('Error placing order:', error);
      toast({
        title: 'Error',
        description: error instanceof Error 
          ? error.message 
          : 'Failed to place order. Please try again.',
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    } finally {
      setLoading(false);
    }
  };

  if (backendAvailable === false) {
    return (
      <Alert status="error" mb={4} flexDirection="column" alignItems="center" justifyContent="center" textAlign="center" height="200px">
        <AlertIcon boxSize="40px" mr={0} />
        <AlertTitle mt={4} mb={1} fontSize="lg">
          Backend Server Unavailable
        </AlertTitle>
        <AlertDescription maxWidth="sm">
          Unable to connect to the backend server. Please make sure the Azure Functions backend is running.
          {retryCount < MAX_RETRIES ? (
            <Text mt={2}>Retrying connection... (Attempt {retryCount + 1} of {MAX_RETRIES})</Text>
          ) : (
            <Text mt={2}>Maximum retry attempts reached.</Text>
          )}
        </AlertDescription>
        <ChakraButton
          colorScheme="blue"
          mt={4}
          onClick={() => {
            setRetryCount(0);
            checkBackend();
          }}
        >
          Retry Connection
        </ChakraButton>
      </Alert>
    );
  }

  return (
    <Box>
      <Text fontSize="2xl" mb={6}>Place New Order</Text>
      <form onSubmit={handleSubmit}>
        <VStack spacing={4} align="stretch">
          <FormControl isRequired>
            <FormLabel>Pizza Size</FormLabel>
            <Select
              value={order.size}
              onChange={(e) => setOrder({ ...order, size: e.target.value })}
              placeholder="Select size"
            >
              <option value="small">Small</option>
              <option value="medium">Medium</option>
              <option value="large">Large</option>
            </Select>
          </FormControl>

          <FormControl isRequired>
            <FormLabel>Toppings</FormLabel>
            <Select
              multiple
              value={order.toppings}
              onChange={(e) => {
                const selectedOptions = Array.from(e.target.selectedOptions, option => option.value);
                setOrder({ ...order, toppings: selectedOptions });
              }}
            >
              <option value="pepperoni">Pepperoni</option>
              <option value="mushrooms">Mushrooms</option>
              <option value="onions">Onions</option>
              <option value="sausage">Sausage</option>
              <option value="bacon">Bacon</option>
              <option value="extra-cheese">Extra Cheese</option>
            </Select>
          </FormControl>

          <FormControl isRequired>
            <FormLabel>Delivery Address</FormLabel>
            <Input
              value={order.address}
              onChange={(e) => setOrder({ ...order, address: e.target.value })}
              placeholder="Enter delivery address"
            />
          </FormControl>

          <FormControl isRequired>
            <FormLabel>Phone Number</FormLabel>
            <Input
              value={order.phone}
              onChange={(e) => setOrder({ ...order, phone: e.target.value })}
              placeholder="Enter phone number"
              type="tel"
            />
          </FormControl>

          <Button
            type="submit"
            colorScheme="blue"
            isLoading={loading}
            loadingText="Placing Order..."
          >
            Place Order
          </Button>
        </VStack>
      </form>
    </Box>
  );
};

export default NewOrder; 