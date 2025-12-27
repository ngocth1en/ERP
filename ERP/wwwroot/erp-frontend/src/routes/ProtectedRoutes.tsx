import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuthStore } from "../auth/auth.store";

export type SystemCode = "ASM" | "QCM" | "HRM";

interface ProtectedRouteProps {
  children: ReactNode;
  system?: SystemCode;
}

const ProtectedRoute = ({ children, system }: ProtectedRouteProps) => {
  const { isAuthenticated, systems } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (system && !systems.includes(system)) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
