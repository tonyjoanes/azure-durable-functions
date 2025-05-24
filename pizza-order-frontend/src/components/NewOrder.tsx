import React, { useState, useEffect, useCallback } from 'react';
import {
  Box,
  Button,
  FormControl,
  FormLabel,
  Input,
  VStack,
  HStack,
  useToast,
  Text,
  Alert,
  AlertIcon,
  AlertTitle,
  AlertDescription,
  Button as ChakraButton,
  SimpleGrid,
  Card,
  CardBody,
  Badge,
  Checkbox,
  Radio,
  RadioGroup,
  Stack,
  Divider,
  Heading,
  Flex,
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

const pizzaSizes = [
  { value: 'small', label: 'Small', price: '$12.99', description: '10" - Perfect for 1-2 people', diameter: '60px' },
  { value: 'medium', label: 'Medium', price: '$16.99', description: '12" - Great for 2-3 people', diameter: '80px' },
  { value: 'large', label: 'Large', price: '$20.99', description: '14" - Feeds 3-4 people', diameter: '100px' },
];

const toppingsOptions = [
  { value: 'pepperoni', label: 'Pepperoni', emoji: '🍕' },
  { value: 'mushrooms', label: 'Mushrooms', emoji: '🍄' },
  { value: 'onions', label: 'Onions', emoji: '🧅' },
  { value: 'sausage', label: 'Sausage', emoji: '🌭' },
  { value: 'bacon', label: 'Bacon', emoji: '🥓' },
  { value: 'extra-cheese', label: 'Extra Cheese', emoji: '🧀' },
  { value: 'peppers', label: 'Bell Peppers', emoji: '🫑' },
  { value: 'olives', label: 'Black Olives', emoji: '🫒' },
];

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

  const checkBackend = useCallback(async () => {
    try {
      const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.health}`, {
        method: 'GET',
        headers: {
          'Accept': 'application/json'
        }
      });
      setBackendAvailable(response.ok);
      if (response.ok) {
        setRetryCount(0);
      }
    } catch (error) {
      console.error('Backend health check failed:', error);
      setBackendAvailable(false);
      
      if (retryCount < MAX_RETRIES) {
        setTimeout(() => {
          setRetryCount(prev => prev + 1);
        }, RETRY_DELAY);
      }
    }
  }, [retryCount]);

  useEffect(() => {
    checkBackend();
  }, [checkBackend]);

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
        title: 'Order Placed! 🍕',
        description: `Your delicious pizza is on the way! Order ID: ${data.id}`,
        status: 'success',
        duration: 5000,
        isClosable: true,
      });

      setOrder({
        size: '',
        toppings: [],
        address: '',
        phone: '',
      });
    } catch (error) {
      console.error('Error placing order:', error);
      toast({
        title: 'Oops! Something went wrong',
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

  const handleToppingChange = (toppingValue: string, isChecked: boolean) => {
    setOrder(prev => ({
      ...prev,
      toppings: isChecked 
        ? [...prev.toppings, toppingValue]
        : prev.toppings.filter(t => t !== toppingValue)
    }));
  };

  const getSelectedSizeDetails = () => {
    return pizzaSizes.find(size => size.value === order.size);
  };

  const calculateTotal = () => {
    const sizePrice = getSelectedSizeDetails()?.price || '$0.00';
    const basePrice = parseFloat(sizePrice.replace('$', ''));
    const toppingsPrice = order.toppings.length * 1.50; // $1.50 per topping
    return `$${(basePrice + toppingsPrice).toFixed(2)}`;
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

  const isFormValid = order.size && order.address && order.phone;

  return (
    <Box>
      <VStack spacing={8} align="stretch">
        <Box textAlign="center">
          <Heading size="xl" color="orange.500" mb={2}>
            🍕 Craft Your Perfect Pizza
          </Heading>
          <Text color="gray.600" fontSize="lg">
            Fresh ingredients, made to order, delivered hot!
          </Text>
        </Box>

        <form onSubmit={handleSubmit}>
          <SimpleGrid columns={{ base: 1, lg: 2 }} spacing={8}>
            {/* Left Column - Pizza Configuration */}
            <VStack spacing={6} align="stretch">
              
              {/* Size Selection */}
              <Card>
                <CardBody>
                  <FormControl isRequired>
                    <FormLabel fontSize="lg" fontWeight="bold" color="gray.700" mb={4}>
                      Choose Your Size
                    </FormLabel>
                    <RadioGroup value={order.size} onChange={(value) => setOrder({ ...order, size: value })}>
                      <VStack spacing={4} align="stretch">
                        {pizzaSizes.map((size) => (
                          <Box key={size.value}>
                            <Radio value={size.value} colorScheme="orange">
                              <HStack spacing={4} align="center">
                                <Box
                                  width={size.diameter}
                                  height={size.diameter}
                                  borderRadius="50%"
                                  bg="orange.100"
                                  border="3px solid"
                                  borderColor={order.size === size.value ? "orange.500" : "orange.200"}
                                  display="flex"
                                  alignItems="center"
                                  justifyContent="center"
                                  transition="all 0.2s"
                                >
                                  <Text fontSize="2xl">🍕</Text>
                                </Box>
                                <Box>
                                  <Text fontWeight="bold">{size.label}</Text>
                                  <Text fontSize="sm" color="gray.600">{size.description}</Text>
                                  <Badge colorScheme="green" variant="subtle">{size.price}</Badge>
                                </Box>
                              </HStack>
                            </Radio>
                          </Box>
                        ))}
                      </VStack>
                    </RadioGroup>
                  </FormControl>
                </CardBody>
              </Card>

              {/* Toppings Selection */}
              <Card>
                <CardBody>
                  <FormControl>
                    <FormLabel fontSize="lg" fontWeight="bold" color="gray.700" mb={4}>
                      Select Your Toppings
                      <Badge ml={2} colorScheme="blue" variant="subtle">+$1.50 each</Badge>
                    </FormLabel>
                    <SimpleGrid columns={2} spacing={3}>
                      {toppingsOptions.map((topping) => (
                        <Box key={topping.value}>
                          <Checkbox
                            isChecked={order.toppings.includes(topping.value)}
                            onChange={(e) => handleToppingChange(topping.value, e.target.checked)}
                            colorScheme="orange"
                          >
                            <HStack spacing={2}>
                              <Text fontSize="lg">{topping.emoji}</Text>
                              <Text fontSize="sm">{topping.label}</Text>
                            </HStack>
                          </Checkbox>
                        </Box>
                      ))}
                    </SimpleGrid>
                  </FormControl>
                </CardBody>
              </Card>

              {/* Delivery Information */}
              <Card>
                <CardBody>
                  <FormLabel fontSize="lg" fontWeight="bold" color="gray.700" mb={4}>
                    Delivery Information
                  </FormLabel>
                  <VStack spacing={4}>
                    <FormControl isRequired>
                      <FormLabel>
                        <Text as="span" mr={2}>📍</Text>
                        Delivery Address
                      </FormLabel>
                      <Input
                        value={order.address}
                        onChange={(e) => setOrder({ ...order, address: e.target.value })}
                        placeholder="123 Main Street, City, State 12345"
                        focusBorderColor="orange.400"
                      />
                    </FormControl>

                    <FormControl isRequired>
                      <FormLabel>
                        <Text as="span" mr={2}>📞</Text>
                        Phone Number
                      </FormLabel>
                      <Input
                        value={order.phone}
                        onChange={(e) => setOrder({ ...order, phone: e.target.value })}
                        placeholder="(555) 123-4567"
                        type="tel"
                        focusBorderColor="orange.400"
                      />
                    </FormControl>
                  </VStack>
                </CardBody>
              </Card>
            </VStack>

            {/* Right Column - Order Summary */}
            <VStack spacing={6} align="stretch">
              <Card bg="gray.50" borderColor="orange.200" borderWidth="2px">
                <CardBody>
                  <Heading size="md" mb={4} color="gray.700">
                    Order Summary
                  </Heading>
                  
                  {order.size ? (
                    <VStack spacing={3} align="stretch">
                      <Box>
                        <Text fontWeight="bold">Size: {getSelectedSizeDetails()?.label}</Text>
                        <Text fontSize="sm" color="gray.600">{getSelectedSizeDetails()?.description}</Text>
                        <Text color="green.600" fontWeight="bold">{getSelectedSizeDetails()?.price}</Text>
                      </Box>
                      
                      <Divider />
                      
                      <Box>
                        <Text fontWeight="bold">Toppings ({order.toppings.length}):</Text>
                        {order.toppings.length > 0 ? (
                          <VStack align="start" spacing={1}>
                            {order.toppings.map(topping => {
                              const toppingInfo = toppingsOptions.find(t => t.value === topping);
                              return (
                                <HStack key={topping} spacing={2}>
                                  <Text fontSize="sm">{toppingInfo?.emoji}</Text>
                                  <Text fontSize="sm">{toppingInfo?.label}</Text>
                                  <Text fontSize="sm" color="green.600">+$1.50</Text>
                                </HStack>
                              );
                            })}
                          </VStack>
                        ) : (
                          <Text fontSize="sm" color="gray.500">No toppings selected</Text>
                        )}
                      </Box>
                      
                      <Divider />
                      
                      <Box>
                        <HStack justify="space-between">
                          <Text fontWeight="bold" fontSize="lg">Total:</Text>
                          <Text fontWeight="bold" fontSize="lg" color="green.600">{calculateTotal()}</Text>
                        </HStack>
                      </Box>
                    </VStack>
                  ) : (
                    <Text color="gray.500" textAlign="center">
                      Select a pizza size to see your order summary
                    </Text>
                  )}
                </CardBody>
              </Card>

              <Button
                type="submit"
                colorScheme="orange"
                size="lg"
                height="60px"
                fontSize="lg"
                isLoading={loading}
                loadingText="Placing Your Order..."
                isDisabled={!isFormValid}
                leftIcon={<Text>✅</Text>}
              >
                Place Order - {order.size ? calculateTotal() : '$0.00'}
              </Button>

              {!isFormValid && (
                <Text fontSize="sm" color="gray.500" textAlign="center">
                  Please fill in all required fields to place your order
                </Text>
              )}
            </VStack>
          </SimpleGrid>
        </form>
      </VStack>
    </Box>
  );
};

export default NewOrder; 