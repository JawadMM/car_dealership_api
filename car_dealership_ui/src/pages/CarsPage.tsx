import React, { useState, useEffect } from "react";
import { carApi } from "../services/api";
import { Car, CreateCarDto, UpdateCarDto, OtpResponseDto } from "../types";
import { useAuth } from "../contexts/AuthContext";
import CarForm from "../components/CarForm";
import CarTable from "../components/CarTable";
import CarFilters from "../components/CarFilters";
import OtpVerification from "../components/OtpVerification";

const CarsPage: React.FC = () => {
  const { user } = useAuth();
  const [cars, setCars] = useState<Car[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [editingCar, setEditingCar] = useState<Car | null>(null);
  const [showOtpVerification, setShowOtpVerification] = useState(false);
  const [otpResponse, setOtpResponse] = useState<OtpResponseDto | null>(null);
  const [pendingUpdateData, setPendingUpdateData] = useState<{
    id: number;
    data: UpdateCarDto;
  } | null>(null);
  const [filters, setFilters] = useState({
    make: "",
    model: "",
    minYear: "",
    maxYear: "",
    maxPrice: "",
    availableOnly: false,
  });

  useEffect(() => {
    loadCars();
  }, []);

  const loadCars = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await carApi.getAll();
      setCars(response.data);
    } catch (err) {
      setError("Failed to load cars");
      console.error("Error loading cars:", err);
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
      if (filters.availableOnly) {
        response = await carApi.getAvailable();
      } else if (Object.keys(searchParams).length > 0) {
        response = await carApi.search(searchParams);
      } else {
        response = await carApi.getAll();
      }

      setCars(response.data);
    } catch (err) {
      setError("Failed to search cars");
      console.error("Error searching cars:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateCar = async (carData: CreateCarDto) => {
    try {
      await carApi.create(carData);
      setShowForm(false);
      loadCars();
    } catch (err) {
      setError("Failed to create car");
      console.error("Error creating car:", err);
    }
  };

  const handleUpdateCar = async (id: number, carData: UpdateCarDto) => {
    try {
      setError(null);
      const otpResult = await carApi.requestUpdateOtp(id, carData);
      setOtpResponse(otpResult.data);
      setPendingUpdateData({ id, data: carData });
      setShowOtpVerification(true);
      setShowForm(false); // Close the form while waiting for OTP
    } catch (err: any) {
      setError("Failed to send OTP for vehicle update. Please try again.");
      console.error("Error requesting update OTP:", err);
    }
  };

  const handleOtpVerify = async (verifyData: any) => {
    try {
      await carApi.verifyUpdateOtp(verifyData);
      setShowOtpVerification(false);
      setPendingUpdateData(null);
      setOtpResponse(null);
      setEditingCar(null);
      loadCars();
      alert("Vehicle updated successfully!");
    } catch (err: any) {
      throw new Error(
        err.response?.data?.message || "Failed to update vehicle"
      );
    }
  };

  const handleOtpResend = async () => {
    if (pendingUpdateData && user) {
      const otpResult = await carApi.requestUpdateOtp(
        pendingUpdateData.id,
        pendingUpdateData.data
      );
      setOtpResponse(otpResult.data);
    }
  };

  const handleOtpCancel = () => {
    setShowOtpVerification(false);
    setOtpResponse(null);
    setPendingUpdateData(null);
    setShowForm(true); // Reopen the form
  };

  const handleDeleteCar = async (id: number) => {
    if (window.confirm("Are you sure you want to delete this car?")) {
      try {
        await carApi.delete(id);
        loadCars();
      } catch (err) {
        setError("Failed to delete car");
        console.error("Error deleting car:", err);
      }
    }
  };

  const handleEditCar = (car: Car) => {
    setEditingCar(car);
    setShowForm(true);
  };

  const handleCloseForm = () => {
    setShowForm(false);
    setEditingCar(null);
  };

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">Cars Management</h1>
        <button className="btn btn-primary" onClick={() => setShowForm(true)}>
          Add New Car
        </button>
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
            availableOnly: false,
          });
          loadCars();
        }}
      />

      {loading ? (
        <div className="loading">Loading cars...</div>
      ) : (
        <CarTable
          cars={cars}
          onEdit={handleEditCar}
          onDelete={handleDeleteCar}
        />
      )}

      {showForm && (
        <CarForm
          car={editingCar}
          onSubmit={
            editingCar
              ? (data) => handleUpdateCar(editingCar.id, data as UpdateCarDto)
              : (data) => handleCreateCar(data as CreateCarDto)
          }
          onClose={handleCloseForm}
        />
      )}

      {showOtpVerification && otpResponse && user && (
        <OtpVerification
          email={user.email}
          purpose="UpdateVehicle"
          otpResponse={otpResponse}
          onVerify={handleOtpVerify}
          onResend={handleOtpResend}
          onCancel={handleOtpCancel}
        />
      )}
    </div>
  );
};

export default CarsPage;
