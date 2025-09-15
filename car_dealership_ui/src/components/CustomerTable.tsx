import React from "react";
import { User } from "../types";

interface CustomerTableProps {
  customers: User[];
  onDelete: (id: string) => void;
}

const CustomerTable: React.FC<CustomerTableProps> = ({
  customers,
  onDelete,
}) => {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div className="card">
      <div className="card-header">
        <h3 className="card-title">Customers ({customers.length})</h3>
      </div>
      <div className="card-body">
        <div className="table-container">
          <table className="table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Phone</th>
                <th>Address</th>
                <th>City</th>
                <th>State</th>
                <th>Zip Code</th>
                <th>Registration Date</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {customers.map((customer) => (
                <tr key={customer.id}>
                  <td>
                    {customer.firstName} {customer.lastName}
                  </td>
                  <td>{customer.email}</td>
                  <td>{customer.phoneNumber || "-"}</td>
                  <td>{customer.address || "-"}</td>
                  <td>{customer.city || "-"}</td>
                  <td>{customer.state || "-"}</td>
                  <td>{customer.zipCode || "-"}</td>
                  <td>{formatDate(customer.dateCreated)}</td>
                  <td>
                    <button
                      className="btn btn-danger btn-sm"
                      onClick={() => onDelete(customer.id)}
                      title="Delete customer account"
                    >
                      Delete Account
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {customers.length === 0 && (
            <div
              style={{ textAlign: "center", padding: "2rem", color: "#6c757d" }}
            >
              No customers found
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default CustomerTable;
