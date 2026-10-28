import { useAuthStore } from "../auth/auth.store";
import AssetHandoverPage from "../features/asset/asset-handover.page";
import AssetScrapPage from "../features/asset/asset.scrapt.page";
import AssetInformationPage from "../features/asset/asset-information.page";
import AssetInventoryPage from "../features/asset/asset-inventory.page";
import AssetModelPage from "../features/asset/asset-model.page";
import AssetClassifyPage from "../features/asset/asset.classify.page";
import LocationPage from "../features/asset/location.page";
import { SystemDataPage, SystemGuidePage, SystemMenuPage, SystemPermissionPage } from "../features/maint/system";
import { UserListPage, UserProfile } from "../features/maint/user";
import AssetLiquidationPage from "../features/asset/asset-liquidation.page";
import AssetRepairPage from "../features/asset/asset-repair.page";
import AssetMaintenancePage from "../features/asset/asset-maintenance.page";
import AssetReportPage from "../features/asset/asset-report.page";
import WarehouseFloorPlanPage from "../features/asset/warehouse-3d.page";


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
       case "/asset/models":
        return <AssetModelPage />; 
        case "/asset/assets":
            return <AssetInformationPage />
        case "/asset/inventory":
            return <AssetInventoryPage />
        case "/asset/handovers":
            return<AssetHandoverPage/>
        case "/asset/scraps":
            return<AssetScrapPage/>
        case "/asset/repair-orders":
            return<AssetRepairPage/>
        case "/asset/liquidations":
            return<AssetLiquidationPage/>
        case "/asset/maintenance":
            return<AssetMaintenancePage/>
        case "/asset/reports/devices":
            return <AssetReportPage />
        case "/asset/reports/group-quantity":
            return <WarehouseFloorPlanPage />
      default:
        return <div>ASM Dashboard</div>;
    }
  }

  return <div>Dashboard</div>;
};

export default MainContent;
