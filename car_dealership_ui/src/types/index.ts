// Car types
export interface Car {
  id: number;
  make: string;
  model: string;
  year: number;
  color: string;
  vin: string;
  price: number;
  mileage: number;
  transmission?: string;
  fuelType?: string;
  isAvailable: boolean;
  dateAdded: string;
  dateSold?: string;
}

export interface CreateCarDto {
  make: string;
  model: string;
  year: number;
  color: string;
  vin: string;
  price: number;
  mileage: number;
  transmission?: string;
  fuelType?: string;
}

export interface UpdateCarDto {
  make: string;
  model: string;
  year: number;
  color: string;
  vin: string;
  price: number;
  mileage: number;
  transmission?: string;
  fuelType?: string;
  isAvailable: boolean;
}

// Customer types
export interface Customer {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  dateCreated: string;
}

export interface CreateCustomerDto {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
}

export interface UpdateCustomerDto {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
}

// Employee types
export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  position: string;
  salary: number;
  hireDate: string;
  terminationDate?: string;
  isActive: boolean;
}

export interface CreateEmployeeDto {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  position: string;
  salary: number;
}

export interface UpdateEmployeeDto {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  position: string;
  salary: number;
  isActive: boolean;
}

// Sale types
export interface Sale {
  id: number;
  carId: number;
  customerId: number;
  employeeId: number;
  salePrice: number;
  saleDate: string;
  paymentMethod?: string;
  notes?: string;
  car?: Car;
  customer?: Customer;
  employee?: Employee;
}

export interface CreateSaleDto {
  carId: number;
  customerId: number;
  employeeId: number;
  salePrice: number;
  paymentMethod?: string;
  notes?: string;
}

export interface UpdateSaleDto {
  salePrice: number;
  paymentMethod?: string;
  notes?: string;
}

// Authentication types
export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  dateCreated: string;
  roles: string[];
}

export interface RegisterDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  role: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiration: string;
  user: User;
}

// OTP types
export interface OtpRequestDto {
  email: string;
  purpose: string;
  metadata?: string;
}

export interface OtpVerifyDto {
  email: string;
  code: string;
  purpose: string;
}

export interface OtpResponseDto {
  success: boolean;
  message: string;
  expiresAt: string;
  attemptsRemaining: number;
}

export interface OtpVerificationResult {
  isValid: boolean;
  message: string;
  token?: string;
  user?: User;
  metadata?: string;
}

// Purchase Request types
export interface PurchaseRequest {
  id: number;
  carId: number;
  customerId: string;
  requestedPrice: number;
  message?: string;
  requestDate: string;
  status: string;
  adminNotes?: string;
  car?: Car;
  customer?: User;
}

export interface CreatePurchaseRequestDto {
  carId: number;
  requestedPrice: number;
  message?: string;
}

export interface UpdatePurchaseRequestDto {
  status: string;
  adminNotes?: string;
}
