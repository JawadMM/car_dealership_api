import React, { useState, useEffect } from "react";
import { useAuth } from "../contexts/AuthContext";
import { purchaseRequestApi } from "../services/api";
import { PurchaseRequest } from "../types";

const CustomerRequestsPage: React.FC = () => {
  const { user } = useAuth();
  const [purchaseRequests, setPurchaseRequests] = useState<PurchaseRequest[]>(
    []
  );
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadRequests();
  }, []);

  const loadRequests = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await purchaseRequestApi.getMyRequests();
      setPurchaseRequests(response.data);
    } catch (err) {
      setError("Failed to load purchase requests");
      console.error("Error loading requests:", err);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(price);
  };

  const getStatusBadge = (status: string) => {
    const statusClasses = {
      Pending: "badge-warning",
      Approved: "badge-success",
      Rejected: "badge-danger",
      Completed: "badge-info",
    };
    return `badge ${
      statusClasses[status as keyof typeof statusClasses] || "badge-secondary"
    }`;
  };

  const getStatusDescription = (status: string) => {
    const descriptions = {
      Pending: "Your request is being reviewed by our team",
      Approved:
        "Your request has been approved! Contact us to complete the purchase",
      Rejected: "Your request was not approved. See admin notes for details",
      Completed: "Purchase completed successfully",
    };
    return (
      descriptions[status as keyof typeof descriptions] || "Status unknown"
    );
  };

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">My Purchase Requests</h1>
        <p>Track the status of your vehicle purchase requests</p>
      </div>

      {error && <div className="error">{error}</div>}

      {loading ? (
        <div className="loading">Loading your requests...</div>
      ) : (
        <div className="card">
          <div className="card-header">
            <h3 className="card-title">
              Purchase Requests ({purchaseRequests.length})
            </h3>
          </div>
          <div className="card-body">
            {purchaseRequests.length > 0 ? (
              <div className="requests-grid">
                {purchaseRequests.map((request) => (
                  <div key={request.id} className="request-card">
                    <div className="request-header">
                      <div className="request-status">
                        <span className={getStatusBadge(request.status)}>
                          {request.status}
                        </span>
                      </div>
                      <div className="request-date">
                        {formatDate(request.requestDate)}
                      </div>
                    </div>

                    <div className="request-vehicle">
                      {request.car ? (
                        <div>
                          <h4>
                            {request.car.year} {request.car.make}{" "}
                            {request.car.model}
                          </h4>
                          <p className="vehicle-details">
                            {request.car.color} • VIN: {request.car.vin} •{" "}
                            {request.car.mileage.toLocaleString()} miles
                          </p>
                          <p className="vehicle-price">
                            Listed Price: {formatPrice(request.car.price)}
                          </p>
                        </div>
                      ) : (
                        <p>Vehicle information not available</p>
                      )}
                    </div>

                    <div className="request-details">
                      <div className="request-offer">
                        <strong>
                          Your Offer: {formatPrice(request.requestedPrice)}
                        </strong>
                      </div>

                      {request.message && (
                        <div className="request-message">
                          <p>
                            <strong>Your Message:</strong>
                          </p>
                          <p>{request.message}</p>
                        </div>
                      )}

                      {request.adminNotes && (
                        <div className="admin-notes">
                          <p>
                            <strong>Dealer Response:</strong>
                          </p>
                          <p>{request.adminNotes}</p>
                        </div>
                      )}

                      <div className="request-status-description">
                        <p className="status-text">
                          {getStatusDescription(request.status)}
                        </p>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="no-requests">
                <div className="no-requests-content">
                  <h3>No Purchase Requests Yet</h3>
                  <p>You haven't submitted any purchase requests.</p>
                  <p>
                    Browse our available vehicles and submit a request to get
                    started!
                  </p>
                  <a href="/dashboard" className="btn btn-primary">
                    Browse Vehicles
                  </a>
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default CustomerRequestsPage;

