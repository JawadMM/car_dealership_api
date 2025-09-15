import React from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Link,
  Navigate,
} from "react-router-dom";
import { AuthProvider, useAuth } from "./contexts/AuthContext";
import CarsPage from "./pages/CarsPage";
import CarDetailsPage from "./pages/CarDetailsPage";
import CustomersPage from "./pages/CustomersPage";
import AdminPurchaseRequestsPage from "./pages/AdminPurchaseRequestsPage";
import AuthPage from "./pages/AuthPage";
import CustomerDashboard from "./pages/CustomerDashboard";
import CustomerRequestsPage from "./pages/CustomerRequestsPage";
import UnauthorizedPage from "./pages/UnauthorizedPage";
import ProtectedRoute from "./components/ProtectedRoute";
import "./App.css";

const AppContent: React.FC = () => {
  const { isAuthenticated, isAdmin, isCustomer, user, logout } = useAuth();

  if (!isAuthenticated) {
    return (
      <Routes>
        <Route path="/login" element={<AuthPage />} />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    );
  }

  return (
    <div className="App">
      <nav className="navbar">
        <div className="nav-container">
          <h1 className="nav-title">Car Dealership Management</h1>
          <div className="nav-links">
            {isAdmin && (
              <>
                <Link to="/cars" className="nav-link">
                  Cars
                </Link>
                <Link to="/customers" className="nav-link">
                  Customers
                </Link>
                <Link to="/admin-requests" className="nav-link">
                  Purchase Requests
                </Link>
              </>
            )}
            {isCustomer && (
              <>
                <Link to="/dashboard" className="nav-link">
                  Browse Cars
                </Link>
                <Link to="/requests" className="nav-link">
                  My Requests
                </Link>
              </>
            )}
            <div className="nav-user">
              <span className="nav-user-name">Welcome, {user?.firstName}!</span>
              <button className="btn btn-secondary btn-sm" onClick={logout}>
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      <main className="main-content">
        <Routes>
          <Route
            path="/"
            element={
              isAdmin ? (
                <Navigate to="/cars" replace />
              ) : (
                <Navigate to="/dashboard" replace />
              )
            }
          />
          <Route path="/login" element={<Navigate to="/" replace />} />

          {/* Admin Routes */}
          <Route
            path="/cars"
            element={
              <ProtectedRoute requireAdmin>
                <CarsPage />
              </ProtectedRoute>
            }
          />
          <Route path="/cars/:id" element={<CarDetailsPage />} />
          <Route
            path="/customers"
            element={
              <ProtectedRoute requireAdmin>
                <CustomersPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin-requests"
            element={
              <ProtectedRoute requireAdmin>
                <AdminPurchaseRequestsPage />
              </ProtectedRoute>
            }
          />

          {/* Customer Routes */}
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute requireCustomer>
                <CustomerDashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/requests"
            element={
              <ProtectedRoute requireCustomer>
                <CustomerRequestsPage />
              </ProtectedRoute>
            }
          />

          {/* Common Routes */}
          <Route path="/unauthorized" element={<UnauthorizedPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </div>
  );
};

function App() {
  return (
    <AuthProvider>
      <Router>
        <AppContent />
      </Router>
    </AuthProvider>
  );
}

export default App;
