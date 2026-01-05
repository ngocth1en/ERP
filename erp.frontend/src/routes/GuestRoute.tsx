import { useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuthStore } from "../auth/auth.store";

interface GuestRouteProps {
  children: ReactNode;
}

const GuestRoute = ({ children }: GuestRouteProps) => {
  const { isAuthenticated } = useAuthStore();
  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    if (isAuthenticated) {
      // 🔒 Đã login mà vào /login → quay lại trang trước
      navigate(-1);
    }
  }, [isAuthenticated, navigate]);

  // 🚫 Không render Login khi đã login
  if (isAuthenticated) {
    return null;
  }

  return <>{children}</>;
};

export default GuestRoute;
