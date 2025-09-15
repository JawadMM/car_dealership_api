# Car Dealership Management System - React Frontend

A modern React TypeScript application for managing a car dealership's inventory, customers, employees, and sales.

## Features

### 🚗 Cars Management

- View all cars in inventory
- Add new cars with detailed specifications
- Edit car information and availability
- Search and filter cars by make, model, year, price
- Mark cars as available/sold

### 👥 Customers Management

- Manage customer database
- Add/edit customer information
- Search customers by name or email
- Track customer contact details and addresses

### 👨‍💼 Employees Management

- Manage employee records
- Track positions, salaries, and employment status
- Filter active/inactive employees
- Employee performance tracking

### 💰 Sales Management

- Create new sales transactions
- Link cars, customers, and employees
- Track payment methods and sale details
- Filter sales by date range, employee, or customer
- Automatic car availability updates

## Technology Stack

- **React 18** with TypeScript
- **React Router** for navigation
- **Axios** for API communication
- **Modern CSS** with responsive design
- **Material Design** inspired UI components

## Getting Started

### Prerequisites

- Node.js (v16 or higher)
- npm or yarn
- The Car Dealership API running on `http://localhost:5274`

### Installation

1. Navigate to the React app directory:

   ```bash
   cd car-dealership-ui
   ```

2. Install dependencies:

   ```bash
   npm install
   ```

3. Start the development server:

   ```bash
   npm start
   ```

4. Open your browser and navigate to `http://localhost:3000`

### API Configuration

The application is configured to connect to the Car Dealership API at `http://localhost:5274`. Make sure the API is running before using the frontend.

## Project Structure

```
src/
├── components/          # Reusable UI components
│   ├── CarForm.tsx     # Car creation/editing form
│   ├── CarTable.tsx    # Cars display table
│   ├── CarFilters.tsx  # Car search filters
│   ├── CustomerForm.tsx
│   ├── CustomerTable.tsx
│   ├── CustomerFilters.tsx
│   ├── EmployeeForm.tsx
│   ├── EmployeeTable.tsx
│   ├── SaleForm.tsx
│   ├── SaleTable.tsx
│   └── SaleFilters.tsx
├── pages/              # Main application pages
│   ├── CarsPage.tsx
│   ├── CustomersPage.tsx
│   ├── EmployeesPage.tsx
│   └── SalesPage.tsx
├── services/           # API service layer
│   └── api.ts
├── types/              # TypeScript type definitions
│   └── index.ts
├── App.tsx             # Main application component
├── App.css             # Global styles
└── index.tsx           # Application entry point
```

## Key Features

### Responsive Design

- Mobile-first approach
- Adaptive layouts for different screen sizes
- Touch-friendly interface

### Real-time Updates

- Automatic data refresh after operations
- Live search and filtering
- Instant feedback on user actions

### Data Validation

- Client-side form validation
- Error handling and user feedback
- Type-safe API communication

### User Experience

- Intuitive navigation
- Modal forms for data entry
- Confirmation dialogs for destructive actions
- Loading states and error messages

## Available Scripts

- `npm start` - Start development server
- `npm build` - Build for production
- `npm test` - Run tests
- `npm eject` - Eject from Create React App

## API Integration

The frontend communicates with the following API endpoints:

- **Cars**: `/api/cars` (GET, POST, PUT, DELETE, search)
- **Customers**: `/api/customers` (GET, POST, PUT, DELETE, search)
- **Employees**: `/api/employees` (GET, POST, PUT, DELETE, active)
- **Sales**: `/api/sales` (GET, POST, PUT, DELETE, filters)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is part of the Car Dealership Management System.
