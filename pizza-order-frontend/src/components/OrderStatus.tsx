import React, { useState } from 'react';
import {
  Box,
  VStack,
  Text,
  Input,
  Button,
  useToast,
  Badge,
  Card,
  CardBody,
  Stack,
  StackDivider,
} from '@chakra-ui/react';
import config from '../config';

interface OrderStatus {
  id: string;
  status: string;
  details: {
    size: string;
    toppings: string[];
    address: string;
    phone: string;
  };
}

const OrderStatus: React.FC = () => {
  const [orderId, setOrderId] = useState('');
  const [orderStatus, setOrderStatus] = useState<OrderStatus | null>(null);
  const [loading, setLoading] = useState(false);
  const toast = useToast();

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'completed':
        return 'green';
      case 'in progress':
        return 'blue';
      case 'failed':
        return 'red';
      default:
        return 'gray';
    }
  };

  const fetchOrderStatus = async () => {
    if (!orderId) {
      toast({
        title: 'Error',
        description: 'Please enter an order ID',
        status: 'error',
        duration: 3000,
        isClosable: true,
      });
      return;
    }

    setLoading(true);
    try {
      const response = await fetch(
        `${config.apiBaseUrl}${config.apiEndpoints.order.status(orderId)}`,
        {
          method: 'GET',
          ...config.corsOptions,
        }
      );

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      setOrderStatus(data);
    } catch (error) {
      console.error('Error fetching order status:', error);
      toast({
        title: 'Error',
        description: 'Failed to fetch order status',
        status: 'error',
        duration: 3000,
        isClosable: true,
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box>
      <Text fontSize="2xl" mb={6}>Track Order Status</Text>
      
      <VStack spacing={4} align="stretch" mb={8}>
        <Input
          placeholder="Enter Order ID"
          value={orderId}
          onChange={(e) => setOrderId(e.target.value)}
        />
        <Button
          colorScheme="blue"
          onClick={fetchOrderStatus}
          isLoading={loading}
          loadingText="Fetching Status..."
        >
          Track Order
        </Button>
      </VStack>

      {orderStatus && (
        <Card>
          <CardBody>
            <Stack divider={<StackDivider />} spacing={4}>
              <Box>
                <Text fontSize="lg" fontWeight="bold">
                  Order #{orderStatus.id}
                </Text>
                <Badge colorScheme={getStatusColor(orderStatus.status)}>
                  {orderStatus.status}
                </Badge>
              </Box>

              <Box>
                <Text fontWeight="bold">Order Details</Text>
                <Text>Size: {orderStatus.details.size}</Text>
                <Text>Toppings: {orderStatus.details.toppings.join(', ')}</Text>
                <Text>Delivery Address: {orderStatus.details.address}</Text>
                <Text>Phone: {orderStatus.details.phone}</Text>
              </Box>
            </Stack>
          </CardBody>
        </Card>
      )}
    </Box>
  );
};

export default OrderStatus; 