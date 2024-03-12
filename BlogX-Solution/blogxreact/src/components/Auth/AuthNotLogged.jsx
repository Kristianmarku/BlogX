import React from "react";
import { useLocation, Navigate, Outlet } from "react-router-dom";

const AuthNotLogged = () => {
  const location = useLocation();

  const isAuthenticated = document.cookie.includes("accessToken");

  return isAuthenticated ? (
    <Navigate to="/home" state={{ from: location }} replace />
  ) : (
    <Outlet />
  );
};

export default AuthNotLogged;
