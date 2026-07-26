import { Button, Space, Dropdown, type MenuProps } from "antd";
import {
    SearchOutlined,
    PlusOutlined,
    EditOutlined,
    DeleteOutlined,
    CheckOutlined,
    UnlockOutlined,
    PrinterOutlined,
    FileExcelOutlined,
    MoreOutlined,
} from "@ant-design/icons";
import { useAuthStore } from "../auth/auth.store";
import { hasPermission } from "../utils/permission";
import { ActionIndex } from "../constants/action";
import { useUiCommandStore } from "../stores/ui-command.store";
import type { UiCommand } from "../stores/ui-command.store";

interface ToolbarProps {
    isMobile?: boolean;
}

const Toolbar = ({ isMobile = false }: ToolbarProps) => {
    const permission = useAuthStore((s) => s.actionPermission);

    // ===== BUTTON DEFINITIONS (logic giữ nguyên) =====
    const buttons: {
        key: UiCommand;
        icon: React.ReactNode;
        label: string;
        disabled: boolean;
        danger?: boolean;
    }[] = [
            {
                key: "QUERY",
                icon: <SearchOutlined />,
                label: "Query",
                disabled: !hasPermission(permission, ActionIndex.Query),
            },
            {
                key: "INSERT",
                icon: <PlusOutlined />,
                label: "Insert",
                disabled: !hasPermission(permission, ActionIndex.Insert),
            },
            {
                key: "MODIFY",
                icon: <EditOutlined />,
                label: "Modify",
                disabled: !hasPermission(permission, ActionIndex.Modify),
            },
            {
                key: "DELETE",
                icon: <DeleteOutlined />,
                label: "Delete",
                danger: true,
                disabled: !hasPermission(permission, ActionIndex.Delete),
            },
            {
                key: "CONFIRM",
                icon: <CheckOutlined />,
                label: "Confirm",
                disabled: !hasPermission(permission, ActionIndex.Confirm),
            },
            {
                key: "UNLOCK",
                icon: <UnlockOutlined />,
                label: "Unlock",
                disabled: !hasPermission(permission, ActionIndex.Unlock),
            },
            {
                key: "PRINT",
                icon: <PrinterOutlined />,
                label: "Print",
                disabled: !hasPermission(permission, ActionIndex.Print),
            },
            {
                key: "EXCEL",
                icon: <FileExcelOutlined />,
                label: "Excel",
                disabled: !hasPermission(permission, ActionIndex.Excel),
            },
        ];

    // ===== MOBILE: Dropdown menu =====
    if (isMobile) {
        const menuItems: MenuProps["items"] = buttons.map((btn) => ({
            key: btn.key,
            icon: btn.icon,
            label: btn.label,
            disabled: btn.disabled,
            danger: btn.danger,
            onClick: () => useUiCommandStore.getState().trigger(btn.key),
        }));

        return (
            <Dropdown menu={{ items: menuItems }} trigger={["click"]}>
                <Button type="primary" icon={<MoreOutlined />} size="small" />
            </Dropdown>
        );
    }

    // ===== DESKTOP: Space wrap (giữ nguyên layout cũ) =====
    return (
        <Space wrap>
            {buttons.map((btn) => (
                <Button
                    key={btn.key}
                    icon={btn.icon}
                    disabled={btn.disabled}
                    danger={btn.danger}
                    onClick={() => useUiCommandStore.getState().trigger(btn.key)}
                >
                    {btn.label}
                </Button>
            ))}
        </Space>
    );
};

export default Toolbar;