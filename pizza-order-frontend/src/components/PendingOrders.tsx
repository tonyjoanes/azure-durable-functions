import React, { useState, useEffect } from 'react';
import {
  Box,
  Text,
  VStack,
  HStack,
  Heading,
  Badge,
  useToast,
  Button,
  Alert,
  AlertIcon,
  AlertTitle,
  AlertDescription,
  Card,
  CardBody,
  SimpleGrid,
  Divider,
} from '@chakra-ui/react';
import config from '../config';

interface PendingOrder {
  id: string;
  status: string;
  size: string;
  toppings: string[];
  address: string;
  phone: string;
  timestamp: string;
}

interface PendingOrdersResponse {
  orders: PendingOrder[];
  count: number;
  timestamp: string;
}

const PendingOrders: React.FC = () => {
  const [orders, setOrders] = useState<PendingOrder[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [confirmingOrders, setConfirmingOrders] = useState<Set<string>>(new Set());
  const toast = useToast();

  const fetchPendingOrders = async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.order.pending}`, {
        method: 'GET',
        headers: {
          'Accept': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data: PendingOrdersResponse = await response.json();
      setOrders(data.orders);
    } catch (error) {
      console.error('Error fetching pending orders:', error);
      setError(error instanceof Error ? error.message : 'Failed to fetch pending orders');
    } finally {
      setLoading(false);
    }
  };

  const handleConfirmOrder = async (orderId: string) => {
    setConfirmingOrders(prev => {
      const newSet = new Set(prev);
      newSet.add(orderId);
      return newSet;
    });

    try {
      const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.order.confirm(orderId)}`, {
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
        description: `Order ${orderId.substring(0, 8)}... has been confirmed successfully!`,
        status: 'success',
        duration: 3000,
        isClosable: true,
      });

      // Refresh the list
      fetchPendingOrders();
    } catch (error) {
      console.error('Error confirming order:', error);
      toast({
        title: 'Error',
        description: error instanceof Error ? error.message : 'Failed to confirm order',
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    } finally {
      setConfirmingOrders(prev => {
        const newSet = new Set(prev);
        newSet.delete(orderId);
        return newSet;
      });
    }
  };

  useEffect(() => {
    fetchPendingOrders();
    // Auto-refresh every 30 seconds
    const interval = setInterval(fetchPendingOrders, 30000);
    return () => clearInterval(interval);
  }, []);

  return (
    <Box>
      <HStack justify="space-between" mb={6}>
        <Heading size="lg">Pending Orders ({orders.length})</Heading>
        <Button
          colorScheme="blue"
          onClick={fetchPendingOrders}
          isLoading={loading}
          loadingText="Refreshing..."
        >
          Refresh
        </Button>
      </HStack>

      {error && (
        <Alert status="error" mb={4}>
          <AlertIcon />
          <AlertTitle>Error</AlertTitle>
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {loading && orders.length === 0 ? (
        <Text>Loading pending orders...</Text>
      ) : orders.length === 0 ? (
        <Alert status="info">
          <AlertIcon />
          <AlertTitle>No Pending Orders</AlertTitle>
          <AlertDescription>There are currently no orders waiting for approval.</AlertDescription>
        </Alert>
      ) : (
        <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} spacing={4}>
          {orders.map((order) => (
            <Card key={order.id} variant="outline">
              <CardBody>
                <VStack align="stretch" spacing={3}>
                  <Box>
                    <Text fontWeight="bold" fontSize="sm" color="gray.600">Order ID</Text>
                    <Text fontSize="sm" fontFamily="mono">{order.id.substring(0, 8)}...</Text>
                  </Box>
                  
                  <Divider />
                  
                  <Box>
                    <Badge colorScheme="yellow" mb={2}>
                      {order.status.toUpperCase()}
                    </Badge>
                    <Text fontWeight="bold">Pizza Details:</Text>
                    <Text>Size: {order.size}</Text>
                    <Text>Toppings: {order.toppings.join(', ') || 'None'}</Text>
                  </Box>

                  <Box>
                    <Text fontWeight="bold">Delivery:</Text>
                    <Text fontSize="sm">{order.address}</Text>
                    <Text fontSize="sm">{order.phone}</Text>
                  </Box>

                  <Box>
                    <Text fontWeight="bold" fontSize="sm">Order Time:</Text>
                    <Text fontSize="sm">{new Date(order.timestamp).toLocaleString()}</Text>
                  </Box>

                  <Button
                    colorScheme="green"
                    size="sm"
                    onClick={() => handleConfirmOrder(order.id)}
                    isLoading={confirmingOrders.has(order.id)}
                    loadingText="Confirming..."
                  >
                    Confirm Order
                  </Button>
                </VStack>
              </CardBody>
            </Card>
          ))}
        </SimpleGrid>
      )}
    </Box>
  );
};

export default PendingOrders; 