import React, { useState } from 'react';
import {
  Box,
  Text,
  VStack,
  Heading,
  Badge,
  useToast,
  Button,
  Alert,
  AlertIcon,
  AlertTitle,
  AlertDescription,
} from '@chakra-ui/react';
import config from '../config';

interface OrderStatusData {
  id: string;
  status: string;
  size: string;
  toppings: string[];
  address: string;
  phone: string;
  timestamp: string;
}

const OrderStatus: React.FC = () => {
  const [orderId, setOrderId] = useState<string>('');
  const [order, setOrder] = useState<OrderStatusData | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const toast = useToast();

  const checkOrderStatus = async () => {
    if (!orderId) return;

    setLoading(true);
    setError(null);

    try {
      const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.order.status(orderId)}`, {
        method: 'GET',
        headers: {
          'Accept': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      setOrder(data);
    } catch (error) {
      console.error('Error checking order status:', error);
      setError(error instanceof Error ? error.message : 'Failed to check order status');
    } finally {
      setLoading(false);
    }
  };

  const handleConfirmOrder = async () => {
    if (!order) return;

    try {
      const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.order.confirm(order.id)}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      toast({
        title: 'Order Confirmed',
        description: 'Your order has been confirmed successfully!',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });

      // Refresh order status
      checkOrderStatus();
    } catch (error) {
      console.error('Error confirming order:', error);
      toast({
        title: 'Error',
        description: error instanceof Error ? error.message : 'Failed to confirm order',
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
  };

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'yellow';
      case 'confirmed':
        return 'green';
      case 'preparing':
        return 'blue';
      case 'out_for_delivery':
        return 'purple';
      case 'delivered':
        return 'green';
      default:
        return 'gray';
    }
  };

  return (
    <Box>
      <Heading size="lg" mb={6}>Check Order Status</Heading>
      
      <VStack spacing={4} align="stretch">
        <Box>
          <Text mb={2}>Enter Order ID:</Text>
          <input
            type="text"
            value={orderId}
            onChange={(e) => setOrderId(e.target.value)}
            placeholder="Enter your order ID"
            style={{
              padding: '8px',
              width: '100%',
              marginBottom: '16px',
              border: '1px solid #ccc',
              borderRadius: '4px'
            }}
          />
          <Button
            colorScheme="blue"
            onClick={checkOrderStatus}
            isLoading={loading}
            loadingText="Checking..."
          >
            Check Status
          </Button>
        </Box>

        {error && (
          <Alert status="error">
            <AlertIcon />
            <AlertTitle>Error</AlertTitle>
            <AlertDescription>{error}</AlertDescription>
          </Alert>
        )}

        {order && (
          <Box p={4} borderWidth={1} borderRadius="md">
            <VStack align="stretch" spacing={3}>
              <Box>
                <Text fontWeight="bold">Order ID:</Text>
                <Text>{order.id}</Text>
              </Box>
              
              <Box>
                <Text fontWeight="bold">Status:</Text>
                <Badge colorScheme={getStatusColor(order.status)}>
                  {order.status}
                </Badge>
              </Box>

              <Box>
                <Text fontWeight="bold">Pizza Details:</Text>
                <Text>Size: {order.size}</Text>
                <Text>Toppings: {order.toppings.join(', ')}</Text>
              </Box>

              <Box>
                <Text fontWeight="bold">Delivery Details:</Text>
                <Text>Address: {order.address}</Text>
                <Text>Phone: {order.phone}</Text>
              </Box>

              <Box>
                <Text fontWeight="bold">Order Time:</Text>
                <Text>{new Date(order.timestamp).toLocaleString()}</Text>
              </Box>

              {order.status === 'pending' && (
                <Button
                  colorScheme="green"
                  onClick={handleConfirmOrder}
                >
                  Confirm Order
                </Button>
              )}
            </VStack>
          </Box>
        )}
      </VStack>
    </Box>
  );
};

export default OrderStatus; 