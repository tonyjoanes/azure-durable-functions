# Pizza Ordering System Frontend

This is the frontend application for the Pizza Ordering System, built with React, TypeScript, and Chakra UI.

## Features

- Place new pizza orders
- Track order status
- View order details
- Real-time status updates

## Prerequisites

- Node.js (v14 or higher)
- npm (v6 or higher)

## Setup

1. Install dependencies:
   ```bash
   npm install
   ```

2. Start the development server:
   ```bash
   npm start
   ```

The application will be available at http://localhost:3000

## Development

The frontend communicates with the Azure Durable Functions backend running at http://localhost:7297. Make sure the backend is running before using the frontend application.

## Project Structure

- `src/`
  - `components/` - React components
    - `NewOrder.tsx` - Component for placing new orders
    - `OrderStatus.tsx` - Component for tracking order status
  - `App.tsx` - Main application component
  - `index.tsx` - Application entry point

## Available Scripts

- `npm start` - Runs the app in development mode
- `npm build` - Builds the app for production
- `npm test` - Runs the test suite
- `npm eject` - Ejects from Create React App 