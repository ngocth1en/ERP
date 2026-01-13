import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuthStore } from "../auth/auth.store";

interface GuestRouteProps {
  children: ReactNode;
}

const GuestRoute = ({ children }: GuestRouteProps) => {
  const { isAuthenticated, systemCode } = useAuthStore();
  const switchSystem = useAuthStore((s) => s.switchSystem);

  const navigate = useNavigate();
  console.log(systemCode);
  useEffect(() => {
    if (isAuthenticated) {
      // 🔒 Đã login mà vào /login → quay lại trang trước
      if (systemCode === undefined)
        navigate(-1);
      else {
        switchSystem(systemCode);
        navigate(`/${systemCode.toLowerCase()}`);
      }
    }
  }, [isAuthenticated, navigate]);

  // 🚫 Không render Login khi đã login
  if (isAuthenticated) {
    return null;
  }

  return <>{children}</>;
};

export default GuestRoute;
