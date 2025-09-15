import axios from "axios";
import {
  Car,
  CreateCarDto,
  UpdateCarDto,
  Customer,
  CreateCustomerDto,
  UpdateCustomerDto,
  Employee,
  CreateEmployeeDto,
  UpdateEmployeeDto,
  Sale,
  CreateSaleDto,
  UpdateSaleDto,
  User,
  RegisterDto,
  LoginDto,
  AuthResponse,
  PurchaseRequest,
  CreatePurchaseRequestDto,
  UpdatePurchaseRequestDto,
  OtpRequestDto,
  OtpVerifyDto,
  OtpResponseDto,
  OtpVerificationResult,
} from "../types";

const API_BASE_URL = "http://localhost:5274/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Add request interceptor to include auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Add response interceptor to handle auth errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

// Car API (Updated for OTP on updates)
export const carApi = {
  getAll: () => api.get<Car[]>("/cars"),
  getById: (id: number) => api.get<Car>(`/cars/${id}`),
  getAvailable: () => api.get<Car[]>("/cars/available"),
  search: (params: {
    make?: string;
    model?: string;
    minYear?: number;
    maxYear?: number;
    maxPrice?: number;
  }) => api.get<Car[]>("/cars/search", { params }),
  create: (data: CreateCarDto) => api.post<Car>("/cars", data),

  // OTP-based vehicle updates
  requestUpdateOtp: (id: number, data: UpdateCarDto) =>
    api.post<OtpResponseDto>(`/cars/${id}/update/request-otp`, data),
  verifyUpdateOtp: (data: OtpVerifyDto) =>
    api.put<Car>("/cars/update/verify-otp", data),

  delete: (id: number) => api.delete(`/cars/${id}`),
};

// Customer API (Updated to use registered users)
export const customerApi = {
  getAll: () => api.get<User[]>("/customers"),
  getById: (id: string) => api.get<User>(`/customers/${id}`),
  search: (params: { name?: string; email?: string }) =>
    api.get<User[]>("/customers/search", { params }),
  delete: (id: string) => api.delete(`/customers/${id}`),
};

// Employee API
export const employeeApi = {
  getAll: () => api.get<Employee[]>("/employees"),
  getById: (id: number) => api.get<Employee>(`/employees/${id}`),
  getActive: () => api.get<Employee[]>("/employees/active"),
  create: (data: CreateEmployeeDto) => api.post<Employee>("/employees", data),
  update: (id: number, data: UpdateEmployeeDto) =>
    api.put<Employee>(`/employees/${id}`, data),
  delete: (id: number) => api.delete(`/employees/${id}`),
};

// Sale API
export const saleApi = {
  getAll: () => api.get<Sale[]>("/sales"),
  getById: (id: number) => api.get<Sale>(`/sales/${id}`),
  getByDateRange: (startDate: string, endDate: string) =>
    api.get<Sale[]>("/sales/date-range", { params: { startDate, endDate } }),
  getByEmployee: (employeeId: number) =>
    api.get<Sale[]>(`/sales/employee/${employeeId}`),
  getByCustomer: (customerId: number) =>
    api.get<Sale[]>(`/sales/customer/${customerId}`),
  create: (data: CreateSaleDto) => api.post<Sale>("/sales", data),
  update: (id: number, data: UpdateSaleDto) =>
    api.put<Sale>(`/sales/${id}`, data),
  delete: (id: number) => api.delete(`/sales/${id}`),
};

// Auth API (Updated for OTP)
export const authApi = {
  // OTP-based registration
  requestRegisterOtp: (data: RegisterDto) =>
    api.post<OtpResponseDto>("/auth/register/request-otp", data),
  verifyRegisterOtp: (data: OtpVerifyDto) =>
    api.post<AuthResponse>("/auth/register/verify-otp", data),

  // OTP-based login
  requestLoginOtp: (data: LoginDto) =>
    api.post<OtpResponseDto>("/auth/login/request-otp", data),
  verifyLoginOtp: (data: OtpVerifyDto) =>
    api.post<AuthResponse>("/auth/login/verify-otp", data),

  // User management
  getUsers: () => api.get<User[]>("/auth/users"),
  getUser: (userId: string) => api.get<User>(`/auth/users/${userId}`),
  updateUser: (userId: string, data: User) =>
    api.put(`/auth/users/${userId}`, data),
  deleteUser: (userId: string) => api.delete(`/auth/users/${userId}`),
};

// Purchase Request API (Updated for OTP)
export const purchaseRequestApi = {
  getAll: () => api.get<PurchaseRequest[]>("/purchaserequests"),
  getPending: () => api.get<PurchaseRequest[]>("/purchaserequests/pending"),
  getMyRequests: () =>
    api.get<PurchaseRequest[]>("/purchaserequests/my-requests"),
  getById: (id: number) => api.get<PurchaseRequest>(`/purchaserequests/${id}`),

  // OTP-based purchase request creation
  requestOtp: (data: CreatePurchaseRequestDto) =>
    api.post<OtpResponseDto>("/purchaserequests/request-otp", data),
  verifyOtp: (data: OtpVerifyDto) =>
    api.post<PurchaseRequest>("/purchaserequests/verify-otp", data),

  update: (id: number, data: UpdatePurchaseRequestDto) =>
    api.put<PurchaseRequest>(`/purchaserequests/${id}`, data),
  delete: (id: number) => api.delete(`/purchaserequests/${id}`),
};

// OTP API
export const otpApi = {
  generate: (data: OtpRequestDto) =>
    api.post<OtpResponseDto>("/otp/generate", data),
  verify: (data: OtpVerifyDto) =>
    api.post<OtpVerificationResult>("/otp/verify", data),
  validate: (email: string, purpose: string) =>
    api.get<boolean>(`/otp/validate?email=${email}&purpose=${purpose}`),
};

export default api;
