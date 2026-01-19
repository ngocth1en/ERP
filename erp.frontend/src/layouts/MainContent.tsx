import { useAuthStore } from "../auth/auth.store";
import AssetClassifyPage from "../features/asset/asset.classify.page";
import LocationPage from "../features/asset/location.page";
import { SystemDataPage, SystemGuidePage, SystemMenuPage, SystemPermissionPage } from "../features/maint/system";
import { UserListPage, UserProfile } from "../features/maint/user";

const MainContent = () => {
  const { systemCode, currentMenuCode } = useAuthStore();
  // MAINT
  if (systemCode === "MAINT") {
    switch (currentMenuCode) {
      case "/maint/user/profile":
        return <UserProfile />;
      case "/maint/help":
        return <SystemGuidePage />;
      case "/maint/system/users":
        return <UserListPage />
      case "/maint/system/config":
        return <SystemDataPage />
      case "/maint/system/catalogs":
        return <SystemMenuPage />
      case "/maint/permission/action":
        return <SystemPermissionPage />
      case "MAINT_DASHBOARD":
      default:
        return <div>MAINT Dashboard</div>;
    }
  }

  // ASSET
  if (systemCode === "ASSET") {
    switch (currentMenuCode) {
      case "/asset/positions":
        return <LocationPage />;
      case "/asset/categories":
        return <AssetClassifyPage />;
      default:
        return <div>ASM Dashboard</div>;
    }
  }

  return <div>Dashboard</div>;
};

export default MainContent;
