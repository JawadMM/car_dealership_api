import React, { useState, useEffect } from "react";
import { OtpResponseDto, OtpVerifyDto } from "../types";

interface OtpVerificationProps {
  email: string;
  purpose: string;
  otpResponse: OtpResponseDto;
  onVerify: (verifyData: OtpVerifyDto) => Promise<void>;
  onResend: () => Promise<void>;
  onCancel: () => void;
}

const OtpVerification: React.FC<OtpVerificationProps> = ({
  email,
  purpose,
  otpResponse,
  onVerify,
  onResend,
  onCancel,
}) => {
  const [otpCode, setOtpCode] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [timeLeft, setTimeLeft] = useState(0);

  useEffect(() => {
    const expiresAt = new Date(otpResponse.expiresAt);
    const now = new Date();
    const secondsLeft = Math.max(
      0,
      Math.floor((expiresAt.getTime() - now.getTime()) / 1000)
    );
    setTimeLeft(secondsLeft);

    const timer = setInterval(() => {
      setTimeLeft((prev) => {
        if (prev <= 1) {
          clearInterval(timer);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [otpResponse.expiresAt]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (otpCode.length !== 6) {
      setError("Please enter a 6-digit OTP code");
      return;
    }

    setError(null);
    setIsLoading(true);

    try {
      await onVerify({
        email,
        code: otpCode,
        purpose,
      });
    } catch (err: any) {
      setError(
        err.response?.data?.message || err.message || "OTP verification failed"
      );
    } finally {
      setIsLoading(false);
    }
  };

  const handleResend = async () => {
    setError(null);
    setIsLoading(true);

    try {
      await onResend();
      setOtpCode("");
    } catch (err: any) {
      setError(err.response?.data?.message || "Failed to resend OTP");
    } finally {
      setIsLoading(false);
    }
  };

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, "0")}`;
  };

  const handleOtpChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value.replace(/\D/g, "").slice(0, 6);
    setOtpCode(value);
  };

  return (
    <div className="modal-overlay">
      <div className="modal otp-modal">
        <div className="modal-header">
          <h2 className="modal-title">Verify Your Identity</h2>
          <button className="modal-close" onClick={onCancel}>
            ×
          </button>
        </div>

        <div className="otp-content">
          <div className="otp-info">
            <p>We've sent a 6-digit verification code to:</p>
            <strong>{email}</strong>
            <p className="otp-purpose">Purpose: {purpose}</p>
          </div>

          {error && <div className="error-message">{error}</div>}

          <div className="otp-timer">
            {timeLeft > 0 ? (
              <p>
                Code expires in: <strong>{formatTime(timeLeft)}</strong>
              </p>
            ) : (
              <p className="expired">Code has expired</p>
            )}
          </div>

          <form onSubmit={handleSubmit} className="otp-form">
            <div className="form-group">
              <label htmlFor="otpCode" className="form-label">
                Enter 6-digit verification code
              </label>
              <input
                type="text"
                id="otpCode"
                value={otpCode}
                onChange={handleOtpChange}
                className="form-control"
                placeholder="000000"
                maxLength={6}
                required
                autoComplete="one-time-code"
                style={{
                  textAlign: "center",
                  fontSize: "1.5rem",
                  letterSpacing: "0.5rem",
                }}
              />
            </div>

            <div className="otp-digits">
              {Array.from({ length: 6 }, (_, i) => (
                <div
                  key={i}
                  className={`otp-digit ${i < otpCode.length ? "filled" : ""}`}
                >
                  {otpCode[i] || ""}
                </div>
              ))}
            </div>

            <div className="otp-actions">
              <button
                type="submit"
                className="btn btn-primary btn-full"
                disabled={isLoading || otpCode.length !== 6 || timeLeft === 0}
              >
                {isLoading ? "Verifying..." : "Verify Code"}
              </button>

              <button
                type="button"
                className="btn btn-secondary btn-full"
                onClick={handleResend}
                disabled={isLoading || timeLeft > 240}
              >
                {isLoading ? "Sending..." : "Resend Code"}
              </button>

              <button
                type="button"
                className="btn btn-outline btn-full"
                onClick={onCancel}
                disabled={isLoading}
              >
                Cancel
              </button>
            </div>

            <div className="otp-help">
              <p>
                <strong>Attempts remaining:</strong>{" "}
                {otpResponse.attemptsRemaining}
              </p>
              <p className="help-text">
                Check the console output for the OTP code (simulation mode)
              </p>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default OtpVerification;
