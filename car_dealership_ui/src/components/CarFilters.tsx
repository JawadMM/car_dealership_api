import React from "react";

interface CarFiltersProps {
  filters: {
    make: string;
    model: string;
    minYear: string;
    maxYear: string;
    maxPrice: string;
    availableOnly: boolean;
  };
  onFiltersChange: (filters: any) => void;
  onSearch: () => void;
  onClear: () => void;
}

const CarFilters: React.FC<CarFiltersProps> = ({
  filters,
  onFiltersChange,
  onSearch,
  onClear,
}) => {
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    onFiltersChange({
      ...filters,
      [name]: type === "checkbox" ? checked : value,
    });
  };

  return (
    <div className="search-filters">
      <h3 style={{ marginBottom: "1rem", color: "#333" }}>
        Search & Filter Cars
      </h3>
      <div className="filter-row">
        <div className="filter-group">
          <label className="form-label">Make</label>
          <input
            type="text"
            name="make"
            value={filters.make}
            onChange={handleChange}
            className="form-control"
            placeholder="e.g., Toyota"
          />
        </div>
        <div className="filter-group">
          <label className="form-label">Model</label>
          <input
            type="text"
            name="model"
            value={filters.model}
            onChange={handleChange}
            className="form-control"
            placeholder="e.g., Camry"
          />
        </div>
        <div className="filter-group">
          <label className="form-label">Min Year</label>
          <input
            type="number"
            name="minYear"
            value={filters.minYear}
            onChange={handleChange}
            className="form-control"
            placeholder="2020"
            min="1900"
            max="2030"
          />
        </div>
        <div className="filter-group">
          <label className="form-label">Max Year</label>
          <input
            type="number"
            name="maxYear"
            value={filters.maxYear}
            onChange={handleChange}
            className="form-control"
            placeholder="2024"
            min="1900"
            max="2030"
          />
        </div>
        <div className="filter-group">
          <label className="form-label">Max Price</label>
          <input
            type="number"
            name="maxPrice"
            value={filters.maxPrice}
            onChange={handleChange}
            className="form-control"
            placeholder="50000"
            min="0"
            step="1000"
          />
        </div>
        <div className="filter-group">
          <label className="form-label">
            <input
              type="checkbox"
              name="availableOnly"
              checked={filters.availableOnly}
              onChange={handleChange}
            />{" "}
            Available Only
          </label>
        </div>
        <div className="filter-group">
          <button className="btn btn-primary" onClick={onSearch}>
            Search
          </button>
          <button
            className="btn btn-secondary"
            onClick={onClear}
            style={{ marginLeft: "0.5rem" }}
          >
            Clear
          </button>
        </div>
      </div>
    </div>
  );
};

export default CarFilters;

