import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { carApi } from "../services/api";
import { Car } from "../types";
import CarFilters from "../components/CarFilters";

const CustomerDashboard: React.FC = () => {
  const { user } = useAuth();
  const [cars, setCars] = useState<Car[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filters, setFilters] = useState({
    make: "",
    model: "",
    minYear: "",
    maxYear: "",
    maxPrice: "",
    availableOnly: true, // Customers only see available cars
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      const carsResponse = await carApi.getAvailable();
      setCars(carsResponse.data);
    } catch (err) {
      setError("Failed to load data");
      console.error("Error loading data:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async () => {
    try {
      setLoading(true);
      setError(null);

      const searchParams: any = {};
      if (filters.make) searchParams.make = filters.make;
      if (filters.model) searchParams.model = filters.model;
      if (filters.minYear) searchParams.minYear = parseInt(filters.minYear);
      if (filters.maxYear) searchParams.maxYear = parseInt(filters.maxYear);
      if (filters.maxPrice)
        searchParams.maxPrice = parseFloat(filters.maxPrice);

      let response;
      if (Object.keys(searchParams).length > 0) {
        response = await carApi.search(searchParams);
        // Filter to only available cars
        setCars(response.data.filter((car: Car) => car.isAvailable));
      } else {
        response = await carApi.getAvailable();
        setCars(response.data);
      }
    } catch (err) {
      setError("Failed to search cars");
      console.error("Error searching cars:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">Welcome, {user?.firstName}!</h1>
        <p>Browse available vehicles and submit purchase requests</p>
      </div>

      {error && <div className="error">{error}</div>}
      <CarFilters
        filters={filters}
        onFiltersChange={setFilters}
        onSearch={handleSearch}
        onClear={() => {
          setFilters({
            make: "",
            model: "",
            minYear: "",
            maxYear: "",
            maxPrice: "",
            availableOnly: true,
          });
          loadData();
        }}
      />

      {loading ? (
        <div className="loading">Loading cars...</div>
      ) : (
        <div className="card">
          <div className="card-header">
            <h3 className="card-title">Available Cars ({cars.length})</h3>
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
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {cars.map((car) => (
                    <tr key={car.id}>
                      <td>
                        <Link to={`/cars/${car.id}`} className="car-link">
                          <strong>
                            {car.make} {car.model}
                          </strong>
                        </Link>
                      </td>
                      <td>{car.year}</td>
                      <td>{car.color}</td>
                      <td>${car.price.toLocaleString()}</td>
                      <td>{car.mileage.toLocaleString()} mi</td>
                      <td>{car.transmission || "-"}</td>
                      <td>{car.fuelType || "-"}</td>
                      <td>
                        <Link
                          to={`/cars/${car.id}`}
                          className="btn btn-primary btn-sm"
                        >
                          View Details
                        </Link>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              {cars.length === 0 && (
                <div
                  style={{
                    textAlign: "center",
                    padding: "2rem",
                    color: "#6c757d",
                  }}
                >
                  No cars found matching your criteria
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CustomerDashboard;
