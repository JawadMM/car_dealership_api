import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { carApi, purchaseRequestApi } from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import { Car, CreatePurchaseRequestDto, OtpResponseDto } from "../types";
import OtpVerification from "../components/OtpVerification";

const CarDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user, isCustomer } = useAuth();

  const [car, setCar] = useState<Car | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showOtpVerification, setShowOtpVerification] = useState(false);
  const [otpResponse, setOtpResponse] = useState<OtpResponseDto | null>(null);
  const [pendingPurchaseRequest, setPendingPurchaseRequest] =
    useState<CreatePurchaseRequestDto | null>(null);

  useEffect(() => {
    if (id) {
      loadCarDetails();
    }
  }, [id]);

  const loadCarDetails = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await carApi.getById(parseInt(id!));
      setCar(response.data);
    } catch (err) {
      setError("Failed to load car details");
      console.error("Error loading car:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleRequestPurchase = async () => {
    if (!car) return;

    const purchaseData: CreatePurchaseRequestDto = {
      carId: car.id,
      requestedPrice: car.price,
    };

    try {
      setError(null);
      const otpResult = await purchaseRequestApi.requestOtp(purchaseData);
      setOtpResponse(otpResult.data);
      setPendingPurchaseRequest(purchaseData);
      setShowOtpVerification(true);
    } catch (err: any) {
      setError("Failed to send OTP for purchase request. Please try again.");
      console.error("Error requesting purchase OTP:", err);
    }
  };

  const handleOtpVerify = async (verifyData: any) => {
    try {
      await purchaseRequestApi.verifyOtp(verifyData);
      setShowOtpVerification(false);
      setPendingPurchaseRequest(null);
      setOtpResponse(null);
      // Show success message instead of alert
      setError(null);
      // You could add a success state here
    } catch (err: any) {
      throw new Error(
        err.response?.data?.message || "Failed to submit purchase request"
      );
    }
  };

  const handleOtpResend = async () => {
    if (pendingPurchaseRequest) {
      const otpResult = await purchaseRequestApi.requestOtp(
        pendingPurchaseRequest
      );
      setOtpResponse(otpResult.data);
    }
  };

  const handleOtpCancel = () => {
    setShowOtpVerification(false);
    setOtpResponse(null);
    setPendingPurchaseRequest(null);
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(price);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  if (loading) {
    return <div className="loading">Loading car details...</div>;
  }

  if (error && !car) {
    return (
      <div className="error-page">
        <h2>Car Not Found</h2>
        <p>{error}</p>
        <button className="btn btn-primary" onClick={() => navigate(-1)}>
          Go Back
        </button>
      </div>
    );
  }

  if (!car) {
    return (
      <div className="error-page">
        <h2>Car Not Found</h2>
        <button className="btn btn-primary" onClick={() => navigate(-1)}>
          Go Back
        </button>
      </div>
    );
  }

  return (
    <div className="car-details-page">
      <div className="car-details-header">
        <button className="btn btn-secondary" onClick={() => navigate(-1)}>
          ← Back
        </button>
        <div className="car-availability">
          <span
            className={`badge ${
              car.isAvailable ? "badge-success" : "badge-danger"
            }`}
          >
            {car.isAvailable ? "Available" : "Sold"}
          </span>
        </div>
      </div>

      {error && <div className="error">{error}</div>}

      <div className="car-details-content">
        <div className="car-details-main">
          <div className="car-title">
            <h1>
              {car.year} {car.make} {car.model}
            </h1>
            <p className="car-price">{formatPrice(car.price)}</p>
          </div>

          <div className="car-specs-grid">
            <div className="spec-item">
              <label>Color</label>
              <span>{car.color}</span>
            </div>
            <div className="spec-item">
              <label>Year</label>
              <span>{car.year}</span>
            </div>
            <div className="spec-item">
              <label>Mileage</label>
              <span>{car.mileage.toLocaleString()} miles</span>
            </div>
            <div className="spec-item">
              <label>VIN</label>
              <span>{car.vin}</span>
            </div>
            <div className="spec-item">
              <label>Transmission</label>
              <span>{car.transmission || "Not specified"}</span>
            </div>
            <div className="spec-item">
              <label>Fuel Type</label>
              <span>{car.fuelType || "Not specified"}</span>
            </div>
            <div className="spec-item">
              <label>Date Added</label>
              <span>{formatDate(car.dateAdded)}</span>
            </div>
            {car.dateSold && (
              <div className="spec-item">
                <label>Date Sold</label>
                <span>{formatDate(car.dateSold)}</span>
              </div>
            )}
          </div>
        </div>

        {isCustomer && car.isAvailable && (
          <div className="purchase-section">
            <div className="purchase-card">
              <h3>Interested in this vehicle?</h3>
              <p>
                Click below to submit a purchase request at the listed price.
              </p>
              <button
                className="btn btn-primary btn-lg"
                onClick={handleRequestPurchase}
              >
                Request Purchase
              </button>
              <div className="form-text" style={{ marginTop: "0.75rem" }}>
                Listed price: {formatPrice(car.price)}
              </div>
            </div>
          </div>
        )}

        {!car.isAvailable && (
          <div className="sold-notice">
            <h3>This vehicle has been sold</h3>
            <p>This vehicle is no longer available for purchase.</p>
          </div>
        )}
      </div>

      {showOtpVerification && otpResponse && user && (
        <OtpVerification
          email={user.email}
          purpose="PurchaseRequest"
          otpResponse={otpResponse}
          onVerify={handleOtpVerify}
          onResend={handleOtpResend}
          onCancel={handleOtpCancel}
        />
      )}
    </div>
  );
};

export default CarDetailsPage;
