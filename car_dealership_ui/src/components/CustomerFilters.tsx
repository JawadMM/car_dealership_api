import React from "react";

interface CustomerFiltersProps {
  filters: {
    name: string;
    email: string;
  };
  onFiltersChange: (filters: any) => void;
  onSearch: () => void;
  onClear: () => void;
}

const CustomerFilters: React.FC<CustomerFiltersProps> = ({
  filters,
  onFiltersChange,
  onSearch,
  onClear,
}) => {
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    onFiltersChange({
      ...filters,
      [name]: value,
    });
  };

  return (
    <div className="search-filters">
      <h3 style={{ marginBottom: "1rem", color: "#333" }}>Search Customers</h3>
      <div className="filter-row">
        <div className="filter-group">
          <label className="form-label">Name</label>
          <input
            type="text"
            name="name"
            value={filters.name}
            onChange={handleChange}
            className="form-control"
            placeholder="Search by first or last name"
          />
        </div>
        <div className="filter-group">
          <label className="form-label">Email</label>
          <input
            type="email"
            name="email"
            value={filters.email}
            onChange={handleChange}
            className="form-control"
            placeholder="Search by email"
          />
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

export default CustomerFilters;

