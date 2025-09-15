import React, { useState, useEffect } from "react";
import { purchaseRequestApi } from "../services/api";
import { PurchaseRequest, UpdatePurchaseRequestDto } from "../types";

const AdminPurchaseRequestsPage: React.FC = () => {
  const [purchaseRequests, setPurchaseRequests] = useState<PurchaseRequest[]>(
    []
  );
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<string>("all"); // all, pending, approved, rejected

  useEffect(() => {
    loadRequests();
  }, []);

  const loadRequests = async () => {
    try {
      setLoading(true);
      setError(null);

      const response =
        filter === "pending"
          ? await purchaseRequestApi.getPending()
          : await purchaseRequestApi.getAll();

      let requests = response.data;

      // Apply client-side filtering if needed
      if (filter !== "all" && filter !== "pending") {
        requests = requests.filter(
          (r) => r.status.toLowerCase() === filter.toLowerCase()
        );
      }

      setPurchaseRequests(requests);
    } catch (err) {
      setError("Failed to load purchase requests");
      console.error("Error loading requests:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadRequests();
  }, [filter]);

  const handleStatusUpdate = async (
    requestId: number,
    status: string,
    adminNotes?: string
  ) => {
    try {
      const updateData: UpdatePurchaseRequestDto = {
        status,
        adminNotes,
      };

      await purchaseRequestApi.update(requestId, updateData);
      loadRequests(); // Reload to show updated data
    } catch (err) {
      setError("Failed to update request status");
      console.error("Error updating request:", err);
    }
  };

  const handleApprove = (request: PurchaseRequest) => {
    handleStatusUpdate(request.id, "Approved");
  };

  const handleReject = (request: PurchaseRequest) => {
    handleStatusUpdate(request.id, "Rejected");
  };

  // No completion step needed

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

  const getFilteredCount = (status: string) => {
    if (status === "all") return purchaseRequests.length;
    return purchaseRequests.filter(
      (r) => r.status.toLowerCase() === status.toLowerCase()
    ).length;
  };

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">Purchase Requests Management</h1>
        <p>Review and approve customer purchase requests</p>
      </div>

      {error && <div className="error">{error}</div>}

      {/* Filter Tabs */}
      <div className="filter-tabs">
        <button
          className={`filter-tab ${filter === "all" ? "active" : ""}`}
          onClick={() => setFilter("all")}
        >
          All Requests ({getFilteredCount("all")})
        </button>
        <button
          className={`filter-tab ${filter === "pending" ? "active" : ""}`}
          onClick={() => setFilter("pending")}
        >
          Pending ({getFilteredCount("pending")})
        </button>
        <button
          className={`filter-tab ${filter === "approved" ? "active" : ""}`}
          onClick={() => setFilter("approved")}
        >
          Approved ({getFilteredCount("approved")})
        </button>
        <button
          className={`filter-tab ${filter === "rejected" ? "active" : ""}`}
          onClick={() => setFilter("rejected")}
        >
          Rejected ({getFilteredCount("rejected")})
        </button>
        {/* Completed filter removed */}
      </div>

      {loading ? (
        <div className="loading">Loading purchase requests...</div>
      ) : (
        <div className="card">
          <div className="card-header">
            <h3 className="card-title">
              {filter === "all"
                ? "All"
                : filter.charAt(0).toUpperCase() + filter.slice(1)}{" "}
              Purchase Requests ({purchaseRequests.length})
            </h3>
          </div>
          <div className="card-body">
            {purchaseRequests.length > 0 ? (
              <div className="requests-grid">
                {purchaseRequests.map((request) => (
                  <div key={request.id} className="admin-request-card">
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

                    <div className="request-customer">
                      {request.customer ? (
                        <div className="customer-info">
                          <h5>Customer Information</h5>
                          <p>
                            <strong>
                              {request.customer.firstName}{" "}
                              {request.customer.lastName}
                            </strong>
                          </p>
                          <p>Email: {request.customer.email}</p>
                          {request.customer.phoneNumber && (
                            <p>Phone: {request.customer.phoneNumber}</p>
                          )}
                        </div>
                      ) : (
                        <p>Customer information not available</p>
                      )}
                    </div>

                    <div className="request-details">
                      <div className="request-offer">
                        <strong>
                          Customer Offer: {formatPrice(request.requestedPrice)}
                        </strong>
                        <span className="price-difference">
                          {request.car && (
                            <span
                              className={
                                request.requestedPrice < request.car.price
                                  ? "price-lower"
                                  : "price-higher"
                              }
                            >
                              (
                              {request.requestedPrice < request.car.price
                                ? "-"
                                : "+"}
                              {formatPrice(
                                Math.abs(
                                  request.requestedPrice - request.car.price
                                )
                              )}
                              )
                            </span>
                          )}
                        </span>
                      </div>

                      {request.message && (
                        <div className="request-message">
                          <p>
                            <strong>Customer Message:</strong>
                          </p>
                          <p>{request.message}</p>
                        </div>
                      )}

                      {request.adminNotes && (
                        <div className="admin-notes">
                          <p>
                            <strong>Admin Notes:</strong>
                          </p>
                          <p>{request.adminNotes}</p>
                        </div>
                      )}

                      {request.status === "Pending" && (
                        <div className="admin-actions">
                          <button
                            className="btn btn-success btn-sm"
                            onClick={() => handleApprove(request)}
                          >
                            Approve Request
                          </button>
                          <button
                            className="btn btn-danger btn-sm"
                            onClick={() => handleReject(request)}
                          >
                            Reject Request
                          </button>
                        </div>
                      )}

                      {/* No actions for Approved state */}
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="no-requests">
                <div className="no-requests-content">
                  <h3>No {filter === "all" ? "" : filter} Purchase Requests</h3>
                  <p>
                    {filter === "pending"
                      ? "No pending requests at the moment."
                      : `No ${filter} purchase requests found.`}
                  </p>
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminPurchaseRequestsPage;
