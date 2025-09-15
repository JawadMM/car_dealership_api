import React, { useState, useEffect } from "react";
import { Car, CreateCarDto, UpdateCarDto } from "../types";

interface CarFormProps {
  car?: Car | null;
  onSubmit: (data: CreateCarDto | UpdateCarDto) => void;
  onClose: () => void;
}

const CarForm: React.FC<CarFormProps> = ({ car, onSubmit, onClose }) => {
  const [formData, setFormData] = useState({
    make: "",
    model: "",
    year: "",
    color: "",
    vin: "",
    price: "",
    mileage: "",
    transmission: "",
    fuelType: "",
    isAvailable: true,
  });

  useEffect(() => {
    if (car) {
      setFormData({
        make: car.make,
        model: car.model,
        year: car.year.toString(),
        color: car.color,
        vin: car.vin,
        price: car.price.toString(),
        mileage: car.mileage.toString(),
        transmission: car.transmission || "",
        fuelType: car.fuelType || "",
        isAvailable: car.isAvailable,
      });
    }
  }, [car]);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value, type } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]:
        type === "checkbox" ? (e.target as HTMLInputElement).checked : value,
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const submitData = {
      make: formData.make,
      model: formData.model,
      year: parseInt(formData.year),
      color: formData.color,
      vin: formData.vin,
      price: parseFloat(formData.price),
      mileage: parseInt(formData.mileage),
      transmission: formData.transmission || undefined,
      fuelType: formData.fuelType || undefined,
      ...(car && { isAvailable: formData.isAvailable }),
    };

    onSubmit(submitData);
  };

  return (
    <div className="modal-overlay">
      <div className="modal">
        <div className="modal-header">
          <h2 className="modal-title">{car ? "Edit Car" : "Add New Car"}</h2>
          <button className="modal-close" onClick={onClose}>
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label className="form-label">Make *</label>
              <input
                type="text"
                name="make"
                value={formData.make}
                onChange={handleChange}
                className="form-control"
                required
              />
            </div>
            <div className="form-group">
              <label className="form-label">Model *</label>
              <input
                type="text"
                name="model"
                value={formData.model}
                onChange={handleChange}
                className="form-control"
                required
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label className="form-label">Year *</label>
              <input
                type="number"
                name="year"
                value={formData.year}
                onChange={handleChange}
                className="form-control"
                min="1900"
                max="2030"
                required
              />
            </div>
            <div className="form-group">
              <label className="form-label">Color *</label>
              <input
                type="text"
                name="color"
                value={formData.color}
                onChange={handleChange}
                className="form-control"
                required
              />
            </div>
          </div>

          <div className="form-group">
            <label className="form-label">VIN *</label>
            <input
              type="text"
              name="vin"
              value={formData.vin}
              onChange={handleChange}
              className="form-control"
              maxLength={17}
              required
            />
          </div>

          <div className="form-row">
            <div className="form-group">
              <label className="form-label">Price *</label>
              <input
                type="number"
                name="price"
                value={formData.price}
                onChange={handleChange}
                className="form-control"
                min="0"
                step="0.01"
                required
              />
            </div>
            <div className="form-group">
              <label className="form-label">Mileage *</label>
              <input
                type="number"
                name="mileage"
                value={formData.mileage}
                onChange={handleChange}
                className="form-control"
                min="0"
                required
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label className="form-label">Transmission</label>
              <select
                name="transmission"
                value={formData.transmission}
                onChange={handleChange}
                className="form-control"
              >
                <option value="">Select Transmission</option>
                <option value="Automatic">Automatic</option>
                <option value="Manual">Manual</option>
                <option value="CVT">CVT</option>
              </select>
            </div>
            <div className="form-group">
              <label className="form-label">Fuel Type</label>
              <select
                name="fuelType"
                value={formData.fuelType}
                onChange={handleChange}
                className="form-control"
              >
                <option value="">Select Fuel Type</option>
                <option value="Gasoline">Gasoline</option>
                <option value="Diesel">Diesel</option>
                <option value="Electric">Electric</option>
                <option value="Hybrid">Hybrid</option>
              </select>
            </div>
          </div>

          {car && (
            <div className="form-group">
              <label className="form-label">
                <input
                  type="checkbox"
                  name="isAvailable"
                  checked={formData.isAvailable}
                  onChange={handleChange}
                />{" "}
                Available for Sale
              </label>
            </div>
          )}

          <div
            style={{
              display: "flex",
              gap: "1rem",
              justifyContent: "flex-end",
              marginTop: "2rem",
            }}
          >
            <button
              type="button"
              className="btn btn-secondary"
              onClick={onClose}
            >
              Cancel
            </button>
            <button type="submit" className="btn btn-primary">
              {car ? "Update Car" : "Add Car"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CarForm;
