import { useAuthStore } from "../auth/auth.store";
import { UserProfile } from "../features/maint/user";

const MainContent = () => {
  const { systemCode, currentMenuCode } = useAuthStore();
  // MAINT
  if (systemCode === "MAINT") {
    switch (currentMenuCode) {
      case "/maint/user/profile":
        return <UserProfile />;

      case "MAINT_DASHBOARD":
      default:
        return <div>MAINT Dashboard</div>;
    }
  }

  // ASSET
  if (systemCode === "ASSET") {
    switch (currentMenuCode) {
      default:
        return <div>ASM Dashboard</div>;
    }
  }

  return <div>Dashboard</div>;
};

export default MainContent;
