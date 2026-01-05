import { Navigate, useNavigate } from "react-router-dom";
import { useEffect } from "react";
import type { ReactNode } from "react";
import { useAuthStore } from "../auth/auth.store";

export type SystemCode = "ASSET" | "QC" | "MAINT" | "DEVELOP";

interface ProtectedRouteProps {
  children: ReactNode;
  system?: SystemCode;
}

const ProtectedRoute = ({ children, system }: ProtectedRouteProps) => {
  const { isAuthenticated, systems } = useAuthStore();
  const navigate = useNavigate();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (system && !systems.includes(system)) {
    return <Navigate to="/login" replace />;
  }
  if (system && !systems.includes(system)) {
    useEffect(() => {
      navigate(-1); // ⬅ quay lại trang trước đó
    }, [navigate]);

    return null; // không render gì
  }
  return <>{children}</>;
};

export default ProtectedRoute;
