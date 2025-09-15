import React, { useState } from "react";
import { useAuth } from "../contexts/AuthContext";
import { LoginDto, OtpResponseDto } from "../types";
import OtpVerification from "./OtpVerification";

interface LoginFormProps {
  onSwitchToRegister: () => void;
}

const LoginForm: React.FC<LoginFormProps> = ({ onSwitchToRegister }) => {
  const { requestLoginOtp, verifyLoginOtp } = useAuth();
  const [formData, setFormData] = useState<LoginDto>({
    email: "",
    password: "",
  });
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [showOtpVerification, setShowOtpVerification] = useState(false);
  const [otpResponse, setOtpResponse] = useState<OtpResponseDto | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsLoading(true);

    try {
      const otpResult = await requestLoginOtp(formData);
      setOtpResponse(otpResult);
      setShowOtpVerification(true);
    } catch (err: any) {
      setError(
        err.response?.data?.message || "Failed to send OTP. Please try again."
      );
    } finally {
      setIsLoading(false);
    }
  };

  const handleOtpVerify = async (verifyData: any) => {
    await verifyLoginOtp(verifyData);
    setShowOtpVerification(false);
  };

  const handleOtpResend = async () => {
    const otpResult = await requestLoginOtp(formData);
    setOtpResponse(otpResult);
  };

  const handleOtpCancel = () => {
    setShowOtpVerification(false);
    setOtpResponse(null);
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-header">
          <h2>Welcome Back</h2>
          <p>Sign in to your account</p>
        </div>

        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit} className="auth-form">
          <div className="form-group">
            <label htmlFor="email" className="form-label">
              Email Address
            </label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className="form-control"
              required
              placeholder="Enter your email"
            />
          </div>

          <div className="form-group">
            <label htmlFor="password" className="form-label">
              Password
            </label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              className="form-control"
              required
              placeholder="Enter your password"
            />
          </div>

          <button
            type="submit"
            className="btn btn-primary btn-full"
            disabled={isLoading}
          >
            {isLoading ? "Sending OTP..." : "Send Verification Code"}
          </button>
        </form>

        <div className="auth-footer">
          <p>
            Don't have an account?{" "}
            <button
              type="button"
              className="link-button"
              onClick={onSwitchToRegister}
            >
              Sign up here
            </button>
          </p>
        </div>
      </div>

      {showOtpVerification && otpResponse && (
        <OtpVerification
          email={formData.email}
          purpose="Login"
          otpResponse={otpResponse}
          onVerify={handleOtpVerify}
          onResend={handleOtpResend}
          onCancel={handleOtpCancel}
        />
      )}
    </div>
  );
};

export default LoginForm;
