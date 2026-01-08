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
    <Space wrap>
      <Button
        icon={<SearchOutlined />}
        disabled={!hasPermission(permission, ActionIndex.Query)}
        onClick={() => useUiCommandStore.getState().trigger("QUERY")}
      >
        Query
      </Button>

      <Button icon={<PlusOutlined />} disabled={!hasPermission(permission, ActionIndex.Insert)}>
        Insert
      </Button>

      <Button icon={<EditOutlined />} disabled={!hasPermission(permission, ActionIndex.Modify)}>
        Modify
      </Button>

      <Button danger icon={<DeleteOutlined />} disabled={!hasPermission(permission, ActionIndex.Delete)}>
        Delete
      </Button>

      <Button icon={<CheckOutlined />} disabled={!hasPermission(permission, ActionIndex.Confirm)}>
        Confirm
      </Button>

      <Button icon={<UnlockOutlined />} disabled={!hasPermission(permission, ActionIndex.Unlock)}>
        Unlock
      </Button>

      <Button icon={<PrinterOutlined />} disabled={!hasPermission(permission, ActionIndex.Print)}>
        Print
      </Button>

      <Button icon={<FileExcelOutlined />} disabled={!hasPermission(permission, ActionIndex.Excel)}>
        Excel
      </Button>
    </Space>
  );
};

export default Toolbar;
