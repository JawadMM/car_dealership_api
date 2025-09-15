import React, { useState, useEffect } from "react";
import { customerApi } from "../services/api";
import { User } from "../types";
import CustomerTable from "../components/CustomerTable";
import CustomerFilters from "../components/CustomerFilters";

const CustomersPage: React.FC = () => {
  const [customers, setCustomers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filters, setFilters] = useState({
    name: "",
    email: "",
  });

  useEffect(() => {
    loadCustomers();
  }, []);

  const loadCustomers = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await customerApi.getAll();
      setCustomers(response.data);
    } catch (err) {
      setError("Failed to load customers");
      console.error("Error loading customers:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async () => {
    try {
      setLoading(true);
      setError(null);

      const searchParams: any = {};
      if (filters.name) searchParams.name = filters.name;
      if (filters.email) searchParams.email = filters.email;

      let response;
      if (Object.keys(searchParams).length > 0) {
        response = await customerApi.search(searchParams);
      } else {
        response = await customerApi.getAll();
      }

      setCustomers(response.data);
    } catch (err) {
      setError("Failed to search customers");
      console.error("Error searching customers:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteCustomer = async (id: string) => {
    if (
      window.confirm("Are you sure you want to delete this customer account?")
    ) {
      try {
        await customerApi.delete(id);
        loadCustomers();
      } catch (err) {
        setError("Failed to delete customer");
        console.error("Error deleting customer:", err);
      }
    }
  };

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">Registered Customers</h1>
        <p>View and manage customer accounts</p>
      </div>

      {error && <div className="error">{error}</div>}

      <CustomerFilters
        filters={filters}
        onFiltersChange={setFilters}
        onSearch={handleSearch}
        onClear={() => {
          setFilters({ name: "", email: "" });
          loadCustomers();
        }}
      />

      {loading ? (
        <div className="loading">Loading customers...</div>
      ) : (
        <CustomerTable customers={customers} onDelete={handleDeleteCustomer} />
      )}
    </div>
  );
};

export default CustomersPage;
