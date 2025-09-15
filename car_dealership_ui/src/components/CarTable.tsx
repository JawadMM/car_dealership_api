import React from "react";
import { Link } from "react-router-dom";
import { Car } from "../types";

interface CarTableProps {
  cars: Car[];
  onEdit?: (car: Car) => void;
  onDelete?: (id: number) => void;
}

const CarTable: React.FC<CarTableProps> = ({ cars, onEdit, onDelete }) => {
  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(price);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div className="card">
      <div className="card-header">
        <h3 className="card-title">Cars ({cars.length})</h3>
      </div>
      <div className="card-body">
        <div className="table-container">
          <table className="table">
            <thead>
              <tr>
                <th>Vehicle</th>
                <th>Year</th>
                <th>Color</th>
                <th>Price</th>
                <th>Mileage</th>
                <th>Transmission</th>
                <th>Fuel Type</th>
                <th>Status</th>
                <th>Date Added</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {cars.map((car) => (
                <tr key={car.id}>
                  <td>
                    <div>
                      <Link to={`/cars/${car.id}`} className="car-link">
                        <strong>
                          {car.make} {car.model}
                        </strong>
                      </Link>
                    </div>
                  </td>
                  <td>{car.year}</td>
                  <td>{car.color}</td>
                  <td>{formatPrice(car.price)}</td>
                  <td>{car.mileage.toLocaleString()} mi</td>
                  <td>{car.transmission || "-"}</td>
                  <td>{car.fuelType || "-"}</td>
                  <td>
                    <span
                      className={`badge ${
                        car.isAvailable ? "badge-success" : "badge-danger"
                      }`}
                    >
                      {car.isAvailable ? "Available" : "Sold"}
                    </span>
                  </td>
                  <td>{formatDate(car.dateAdded)}</td>
                  <td>
                    <div style={{ display: "flex", gap: "0.5rem" }}>
                      {onEdit && (
                        <button
                          className="btn btn-warning btn-sm"
                          onClick={() => onEdit(car)}
                        >
                          Edit
                        </button>
                      )}
                      {onDelete && (
                        <button
                          className="btn btn-danger btn-sm"
                          onClick={() => onDelete(car.id)}
                        >
                          Delete
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {cars.length === 0 && (
            <div
              style={{ textAlign: "center", padding: "2rem", color: "#6c757d" }}
            >
              No cars found
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default CarTable;
