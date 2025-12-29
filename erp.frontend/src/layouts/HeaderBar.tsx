import { Select } from "antd";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../auth/auth.store";
import type { SystemCode } from "../constants/system";

const systemLabelMap: Record<SystemCode, string> = {
  ASSET: "Quản lý tài sản",
  QC: "Quản lý chất lượng",
  MAINT: "Quản lý hệ thống",
};

const HeaderBar = () => {
  const { systems, systemCode, switchSystem } = useAuthStore();
  const navigate = useNavigate();

  return (
    <Select<SystemCode>
      value={systemCode}
      style={{ width: 220 }}
      onChange={(value) => {
        switchSystem(value);               // ✅ value là SystemCode
        navigate(`/${value.toLowerCase()}`);
      }}
      options={systems.map((s) => ({
        label: systemLabelMap[s],
        value: s,
      }))}
    />
  );
};

export default HeaderBar;
