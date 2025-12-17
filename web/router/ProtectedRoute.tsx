import React from "react";
import { Navigate } from "react-router-dom";
import { useAuthStore } from "../src/features/auth/stores/authStore";

interface ProtectedRouteProps {
  children: React.ReactNode;
  requireRole?: "Author" | "Admin";
}

export default function ProtectedRoute({ children, requireRole }: ProtectedRouteProps) {
  const { isAuthenticated, user } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requireRole && user?.role !== requireRole) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}
