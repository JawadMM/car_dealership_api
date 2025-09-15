# Car Dealership API

A comprehensive .NET 9 ASP.NET Core Web API for a car dealership system with React TypeScript frontend. Features JWT authentication, role-based authorization, OTP security, and complete vehicle management capabilities.

## Features

- **Authentication & Security**

  - JWT Bearer token authentication
  - Role-based authorization (Admin/Customer)
  - OTP (One-Time Password) verification for sensitive operations
  - ASP.NET Core Identity integration

- **Vehicle Management**

  - Full CRUD operations for vehicles
  - Advanced search and filtering
  - Availability tracking
  - Admin-only vehicle updates with OTP verification

- **Purchase System**

  - Customer purchase requests with OTP verification
  - Admin approval/rejection workflow
  - Automatic vehicle status updates upon approval

## Tech Stack

- **Backend**: ASP.NET Core 9, Entity Framework Core
- **Frontend**: React with TypeScript
- **Database**: In-Memory Database (for development)
- **Authentication**: JWT + ASP.NET Core Identity
- **Security**: OTP verification system

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (for React UI)

## Quick Start

### Running the API

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd car-dealership-api
   ```

2. **Restore dependencies and run**

   ```bash
   dotnet restore
   dotnet run
   ```

3. **Access the API**
   - API Base URL: `http://localhost:5274`
   - Swagger UI: `http://localhost:5274/swagger`

### Running the React UI (Optional)

1. **Navigate to UI folder**

   ```bash
   cd car-dealership-ui
   ```

2. **Install dependencies and start**
   ```bash
   npm install
   npm start
   ```

The UI will be available at `http://localhost:3000` and expects the API at `http://localhost:5274`.

## Default Credentials

The system comes pre-seeded with:

- **Admin User**
  - Email: `admin@dealership.com`
  - Password: `Admin123!`
  - Role: Admin

## API Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint                | Description                  | Auth Required |
| ------ | ----------------------- | ---------------------------- | ------------- |
| POST   | `/register/request-otp` | Request OTP for registration | No            |
| POST   | `/register/verify-otp`  | Verify OTP and create user   | No            |
| POST   | `/login/request-otp`    | Request OTP for login        | No            |
| POST   | `/login/verify-otp`     | Verify OTP and get token     | No            |
| GET    | `/users`                | List all users               | Admin         |
| GET    | `/users/{id}`           | Get user by ID               | Auth          |
| PUT    | `/users/{id}`           | Update user profile          | Auth          |
| DELETE | `/users/{id}`           | Delete user                  | Admin         |

### Vehicles (`/api/cars`)

| Method | Endpoint                   | Description             | Auth Required |
| ------ | -------------------------- | ----------------------- | ------------- |
| GET    | `/`                        | List all vehicles       | No            |
| GET    | `/available`               | List available vehicles | No            |
| GET    | `/{id}`                    | Get vehicle by ID       | No            |
| GET    | `/search`                  | Search vehicles         | No            |
| POST   | `/`                        | Create new vehicle      | Admin         |
| POST   | `/{id}/update/request-otp` | Request OTP for update  | Admin         |
| PUT    | `/update/verify-otp`       | Verify OTP and update   | Admin         |
| DELETE | `/{id}`                    | Delete vehicle          | Admin         |

### Purchase Requests (`/api/purchaserequests`)

| Method | Endpoint       | Description                   | Auth Required |
| ------ | -------------- | ----------------------------- | ------------- |
| GET    | `/`            | List all requests             | Admin         |
| GET    | `/pending`     | List pending requests         | Admin         |
| GET    | `/my-requests` | List user's requests          | Customer      |
| GET    | `/{id}`        | Get request by ID             | Auth          |
| POST   | `/request-otp` | Request OTP for purchase      | Customer      |
| POST   | `/verify-otp`  | Verify OTP and create request | Customer      |
| PUT    | `/{id}`        | Approve/reject request        | Admin         |
| DELETE | `/{id}`        | Delete request                | Admin         |

### OTP Management (`/api/otp`)

| Method | Endpoint    | Description        | Auth Required |
| ------ | ----------- | ------------------ | ------------- |
| POST   | `/generate` | Generate OTP       | Varies        |
| POST   | `/verify`   | Verify OTP         | Varies        |
| GET    | `/validate` | Check OTP validity | Varies        |

## Security Features

### OTP Requirements

OTP verification is required for these sensitive operations:

- User registration
- User login
- Creating purchase requests
- Updating vehicle information

### JWT Authentication

- Stateless authentication using JWT tokens
- Tokens stored in client-side localStorage
- Role-based access control (Admin/Customer)

### Security Configuration

- OTP expiration: 5 minutes
- Attempt limits on OTP verification
- Background cleanup of expired OTPs
- CORS configured for local development

## Architecture Decisions

### Database

- **In-Memory Database**: Simplifies setup and testing
- **Data Seeding**: Automatic role and admin user creation

### Authentication Flow

1. User provides credentials
2. System validates and sends OTP
3. User verifies OTP
4. System issues JWT token
5. Token used for subsequent requests

### Purchase Workflow

1. Customer submits purchase request with OTP
2. Admin reviews and approves/rejects
3. Upon approval, vehicle marked as unavailable
4. DateSold automatically set

## Development & Testing

- Use Swagger UI for interactive API testing
- OTP codes are logged to console during development
- In-memory database resets on each application restart
- CORS enabled for local React development

## Notes

- All sensitive operations require OTP verification
- Admin actions automatically update vehicle availability
- Password validation occurs before OTP generation for login
- System designed for demonstration and development purposes
