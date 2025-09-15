import React from "react";
import { Link } from "react-router-dom";

const UnauthorizedPage: React.FC = () => {
  return (
    <div className="unauthorized-page">
      <div className="unauthorized-container">
        <div className="unauthorized-content">
          <h1>403</h1>
          <h2>Access Denied</h2>
          <p>You don't have permission to access this page.</p>
          <div className="unauthorized-actions">
            <Link to="/" className="btn btn-primary">
              Go Home
            </Link>
            <Link to="/login" className="btn btn-secondary">
              Login
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default UnauthorizedPage;

