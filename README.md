# Azure Durable Functions - Pizza Ordering System

This project demonstrates the use of Azure Durable Functions by implementing a pizza ordering system. The system showcases various features of Durable Functions including:
- Orchestration functions
- Activity functions
- Human interaction (approval steps)
- Error handling and retries
- State management

## Features

The pizza ordering system includes the following steps:
1. Order submission
2. Payment processing
3. Kitchen preparation
4. Delivery status updates
5. Order completion

## Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) or [Azurite](https://github.com/Azure/Azurite)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

## Local Development Setup

1. Clone this repository
2. Install the prerequisites listed above
3. Start the Azure Storage Emulator or Azurite:
   - **Azurite (Cross-Platform):**
     - Install Azurite globally:
       ```bash
       npm install -g azurite
       ```
     - Run Azurite with the following command (adjust paths as needed):
       ```bash
       azurite --silent --location c:\azurite --debug c:\azurite\debug.log
       ```
   - **Azure Storage Emulator (Windows Only):**
     - Start the Azure Storage Emulator from the Start menu
4. Open the solution in your preferred IDE
5. Build the solution
6. Run the project locally using:
   ```bash
   func start
   ```

## Project Structure

- `PizzaOrderingSystem/` - Main project directory
  - `Functions/` - Contains all Azure Functions
    - `OrderOrchestrator.cs` - Main orchestration function
    - `PaymentActivity.cs` - Handles payment processing
    - `KitchenActivity.cs` - Manages pizza preparation
    - `DeliveryActivity.cs` - Handles delivery status
  - `Models/` - Data models
  - `Services/` - Business logic services

## Testing the Application

1. Start the application locally
2. Use the provided HTTP trigger endpoint to submit a new order
3. Monitor the orchestration progress in the Azure Functions dashboard
4. Approve/reject orders through the approval endpoint

## Next Steps

- [ ] Set up project structure
- [ ] Implement basic order submission
- [ ] Add payment processing
- [ ] Create kitchen preparation workflow
- [ ] Implement delivery tracking
- [ ] Add human approval steps
- [ ] Implement error handling and retries