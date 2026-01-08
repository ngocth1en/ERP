import { Button, Space } from "antd";
import {
  SearchOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  CheckOutlined,
  UnlockOutlined,
  PrinterOutlined,
  FileExcelOutlined,
} from "@ant-design/icons";
import { useAuthStore } from "../auth/auth.store";
import { hasPermission } from "../utils/permission";
import { ActionIndex } from "../constants/action";
import { useUiCommandStore } from "../stores/ui-command.store";


const Toolbar = () => {
  const permission = useAuthStore((s) => s.actionPermission);

  return (
    <Space>
      <Button disabled={!hasPermission(permission, ActionIndex.Query)} icon={<SearchOutlined />}onClick={() => useUiCommandStore.getState().trigger("QUERY")}>Query</Button>
      <Button disabled={!hasPermission(permission, ActionIndex.Insert)} icon={<PlusOutlined />}>Insert</Button>
      <Button disabled={!hasPermission(permission, ActionIndex.Modify)} icon={<EditOutlined />}>Modify</Button>
      <Button danger disabled={!hasPermission(permission, ActionIndex.Delete)} icon={<DeleteOutlined />}>Delete</Button>
      <Button disabled={!hasPermission(permission, ActionIndex.Confirm)} icon={<CheckOutlined />}>Confirm</Button>
      <Button disabled={!hasPermission(permission, ActionIndex.Unlock)} icon={<UnlockOutlined />}>Unlock</Button>
      <Button disabled={!hasPermission(permission, ActionIndex.Print)} icon={<PrinterOutlined />}>Print</Button>
      <Button disabled={!hasPermission(permission, ActionIndex.Excel)} icon={<FileExcelOutlined />}>Excel</Button>
    </Space>
  );
};

export default Toolbar;
