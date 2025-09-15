import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
} from "react";
import {
  User,
  AuthResponse,
  LoginDto,
  RegisterDto,
  OtpResponseDto,
  OtpVerifyDto,
} from "../types";
import { authApi } from "../services/api";

interface AuthContextType {
  user: User | null;
  token: string | null;
  requestLoginOtp: (credentials: LoginDto) => Promise<OtpResponseDto>;
  verifyLoginOtp: (verifyData: OtpVerifyDto) => Promise<void>;
  requestRegisterOtp: (userData: RegisterDto) => Promise<OtpResponseDto>;
  verifyRegisterOtp: (verifyData: OtpVerifyDto) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isCustomer: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const storedToken = localStorage.getItem("token");
    const storedUser = localStorage.getItem("user");

    if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(JSON.parse(storedUser));
    }
    setIsLoading(false);
  }, []);

  const requestLoginOtp = async (
    credentials: LoginDto
  ): Promise<OtpResponseDto> => {
    try {
      const response = await authApi.requestLoginOtp(credentials);
      return response.data;
    } catch (error) {
      console.error("Login OTP request failed:", error);
      throw error;
    }
  };

  const verifyLoginOtp = async (verifyData: OtpVerifyDto) => {
    try {
      const response = await authApi.verifyLoginOtp(verifyData);
      const { token: newToken, user: newUser } = response.data;

      setToken(newToken);
      setUser(newUser);
      localStorage.setItem("token", newToken);
      localStorage.setItem("user", JSON.stringify(newUser));
    } catch (error) {
      console.error("Login OTP verification failed:", error);
      throw error;
    }
  };

  const requestRegisterOtp = async (
    userData: RegisterDto
  ): Promise<OtpResponseDto> => {
    try {
      const response = await authApi.requestRegisterOtp(userData);
      return response.data;
    } catch (error) {
      console.error("Registration OTP request failed:", error);
      throw error;
    }
  };

  const verifyRegisterOtp = async (verifyData: OtpVerifyDto) => {
    try {
      const response = await authApi.verifyRegisterOtp(verifyData);
      const { token: newToken, user: newUser } = response.data;

      setToken(newToken);
      setUser(newUser);
      localStorage.setItem("token", newToken);
      localStorage.setItem("user", JSON.stringify(newUser));
    } catch (error) {
      console.error("Registration OTP verification failed:", error);
      throw error;
    }
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem("token");
    localStorage.removeItem("user");
  };

  const isAuthenticated = !!user && !!token;
  const isAdmin = user?.roles?.includes("Admin") || false;
  const isCustomer = user?.roles?.includes("Customer") || false;

  const value: AuthContextType = {
    user,
    token,
    requestLoginOtp,
    verifyLoginOtp,
    requestRegisterOtp,
    verifyRegisterOtp,
    logout,
    isLoading,
    isAuthenticated,
    isAdmin,
    isCustomer,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
