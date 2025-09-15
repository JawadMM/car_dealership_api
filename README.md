Car Dealership API (ASP.NET Core 9)

Overview
This repository contains a .NET 9 ASP.NET Core Web API for a car dealership, plus a React TypeScript UI in the car-dealership-ui folder. The API implements JWT authentication, role-based authorization (Admin/Customer), and OTP for sensitive flows (Register, Login, Purchase Request, Update Vehicle). Data uses an in-memory database and is seeded with roles, an Admin user, and sample cars.

How to Run the API

- Prerequisites: .NET 9 SDK
- Start API: from the repo root, run:
  - dotnet restore
  - dotnet run
- Default base URL: http://localhost:5274 (HTTPS also available at https://localhost:7110 if enabled)
- Swagger UI: http://localhost:5274/swagger during development

Seeded Credentials

- Admin user: admin@dealership.com / Admin123!
  - Role: Admin

Authentication and Authorization

- JWT Bearer tokens are required for protected endpoints
- Roles: Admin and Customer
- OTP is required for: Register, Login, Create Purchase Request, Update Vehicle

High-Level Flow

- Register: request OTP with user data, verify OTP to create user and receive token
- Login: validate email/password, then request OTP, verify OTP to receive token
- Purchase Request (Customer): request OTP with car/offer, verify OTP to create request
- Update Vehicle (Admin): request update OTP with new data, verify OTP to apply update
- Approve/Reject (Admin): updates request status; when Approved, car is marked unavailable

Available Endpoints (brief)

- Auth (api/auth)

  - POST /register/request-otp: body RegisterDto → sends OTP
  - POST /register/verify-otp: body OtpVerifyDto → creates user, returns AuthResponse
  - POST /login/request-otp: body LoginDto (email/password validated) → sends OTP
  - POST /login/verify-otp: body OtpVerifyDto → returns AuthResponse
  - GET /users [Admin]: list users
  - GET /users/{id} [Auth]: get user
  - PUT /users/{id} [Auth]: update basic profile info
  - DELETE /users/{id} [Admin]: delete user

- Cars (api/cars)

  - GET /: list all
  - GET /available: list available only
  - GET /{id}: get by id
  - GET /search: query by make/model/year/price
  - POST / [Admin]: create car
  - POST /{id}/update/request-otp [Admin]: request OTP for update with UpdateCarDto
  - PUT /update/verify-otp [Admin]: verify OTP to apply update
  - DELETE /{id} [Admin]: delete car

- Purchase Requests (api/purchaserequests)

  - GET / [Admin]: list all
  - GET /pending [Admin]: list pending
  - GET /my-requests [Customer]: list current user requests
  - GET /{id} [Auth]: get by id; restricted to owner/Admin
  - POST /request-otp [Customer]: request OTP to create a purchase request
  - POST /verify-otp [Customer]: verify OTP to create
  - PUT /{id} [Admin]: update status to Approved/Rejected; if Approved, car becomes unavailable
  - DELETE /{id} [Admin]: delete request

- OTP (api/otp)
  - POST /generate: general OTP helper (if exposed)
  - POST /verify: general OTP verification (if exposed)
  - GET /validate?email=&purpose=: check if valid OTP exists

DTOs (selected)

- RegisterDto: firstName, lastName, email, password, confirmPassword, role
- LoginDto: email, password
- OtpRequestDto: email, purpose, metadata
- OtpVerifyDto: email, code, purpose
- CreatePurchaseRequestDto: carId, requestedPrice, message?
- UpdatePurchaseRequestDto: status, adminNotes?

Design Decisions & Assumptions

- In-Memory Database: simplifies setup and resets on each run. Suitable for demo/testing.
- Identity + Roles: ASP.NET Core Identity drives user management; roles are seeded.
- JWT Auth: stateless API with tokens stored on the client (localStorage in UI).
- OTP Service: implemented with persistence, expiration (5 minutes), attempt limits, and background cleanup. Delivery is simulated via console/log output.
- Password Check Before Login OTP: login endpoint validates email/password before sending OTP.
- Purchase Approval Effect: approving a purchase request marks the associated car as unavailable and sets DateSold.
- Minimal Admin Actions: admin can approve or reject; completion step removed.
- CORS: open policy for local development and the React app.

Running the React UI (optional)

- cd car-dealership-ui
- npm install
- npm start
- The UI expects the API at http://localhost:5274

Notes

- Use Swagger to explore and test endpoints interactively
- OTP codes are printed to the API console/logs

