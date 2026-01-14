import {
    Table,
    Typography,
    Row,
    Col,
    Select,
    Modal,
    Form,
    Input,
    Switch,
    message,
} from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../../stores/ui-command.store";
import { getSystems } from "../../../api/system.api";
import {
    getMenuTreeBySystem,
    createSystemMenu,
    updateMenuStatus,
} from "../../../api/menu.api";

const { Title } = Typography;

/* ======================
   TYPES
====================== */
interface MenuDto {
    key: string;
    label: string;
    isUsing: boolean;
    children?: MenuDto[];
}

const SystemMenuPage = () => {
    /* ======================
       UI COMMAND
    ====================== */
    const command = useUiCommandStore((s) => s.command);
    const clearCommand = useUiCommandStore((s) => s.clear);

    /* ======================
       STATE
    ====================== */
    const [systems, setSystems] = useState<any[]>([]);
    const [systemCode, setSystemCode] = useState<string>();

    const [parentMenus, setParentMenus] = useState<MenuDto[]>([]);
    const [childMenus, setChildMenus] = useState<MenuDto[]>([]);

    const [selectedParent, setSelectedParent] = useState<any>();
    const [selectedChild, setSelectedChild] = useState<MenuDto | null>(null);

    const [loading, setLoading] = useState(false);

    const [modalOpen, setModalOpen] = useState(false);
    const [mode, setMode] = useState<"CREATE" | "EDIT">("CREATE");

    const [highlightKey, setHighlightKey] = useState<string | null>(null);
    const [highlightType, setHighlightType] =
        useState<"CREATE" | "MODIFY" | null>(null);

    const [form] = Form.useForm();

    /* ======================
       LOAD SYSTEM
    ====================== */
    useEffect(() => {
        getSystems().then(setSystems);
    }, []);

    /* ======================
       TOOLBAR COMMAND
    ====================== */
    useEffect(() => {
        if (!command) return;

        switch (command) {
            case "QUERY":
                handleQuery();
                break;
            case "INSERT":
                handleInsert();
                break;
            case "MODIFY":
                handleModify();
                break;
            case "DELETE":
                handleDelete();
                break;
        }

        clearCommand();
    }, [command]);

    /* ======================
       COMMAND HANDLER
    ====================== */

    const handleQuery = async () => {
        if (!systemCode) {
            message.warning("Vui lòng chọn hệ thống");
            return;
        }

        setLoading(true);
        try {
            const data = await getMenuTreeBySystem(systemCode);
            setParentMenus(data);
            setChildMenus([]);
            setSelectedParent(null);
            setSelectedChild(null);
        } finally {
            setLoading(false);
        }
    };

    const handleInsert = () => {
        setMode("CREATE");
        form.resetFields();
        setModalOpen(true);
    };

    const handleModify = () => {
        const target = selectedChild || selectedParent;
        if (!target) {
            message.warning("Vui lòng chọn menu");
            return;
        }

        setMode("EDIT");
        form.setFieldsValue({
            name: target.label,
            isActive: target.isUsing,
        });
        setModalOpen(true);
    };

    const handleDelete = async () => {
        const target = selectedChild || selectedParent;
        if (!target) {
            message.warning("Vui lòng chọn menu");
            return;
        }

        await updateMenuStatus({
            Code: target.key,
            Name:target.label,
            isUsing: false,
        });

        // update UI local
        updateLocalMenu(target.key, false, "MODIFY");
        message.success("Đã ngừng menu");
    };

    /* ======================
       LOCAL UPDATE (NO QUERY)
    ====================== */

    const updateLocalMenu = (
        key: string,
        isUsing: boolean,
        type: "CREATE" | "MODIFY"
    ) => {
        setHighlightKey(key);
        setHighlightType(type);

        setParentMenus((prev) => {
            const copy = [...prev];

            // parent
            const pIndex = copy.findIndex((p) => p.key === key);
            if (pIndex !== -1) {
                const updated = { ...copy[pIndex], isUsing };
                copy.splice(pIndex, 1);
                copy.unshift(updated);
                return copy;
            }

            // child
            for (const p of copy) {
                const cIndex = p.children?.findIndex((c) => c.key === key) ?? -1;
                if (cIndex !== -1 && p.children) {
                    const updatedChild = { ...p.children[cIndex], isUsing };
                    p.children.splice(cIndex, 1);
                    p.children.unshift(updatedChild);

                    if (p.key === selectedParent?.key) {
                        setChildMenus([...p.children]);
                    }
                    break;
                }
            }

            return [...copy];
        });
    };

    /* ======================
       SAVE
    ====================== */
    const onSave = async () => {
        const values = await form.validateFields();

        if (mode === "CREATE") {
            const newKey = crypto.randomUUID();

            const newMenu: MenuDto = {
                key: newKey,
                label: values.name,
                isUsing: true,
            };

            await createSystemMenu({
                code: newKey,
                systemCode: systemCode!,
                groupCode: selectedParent?.key,
                name: values.name,
                route: "",
                orderNo: 1,
            });

            if (selectedParent) {
                const children = [newMenu, ...(selectedParent.children || [])];
                selectedParent.children = children;
                setChildMenus(children);
            } else {
                setParentMenus([newMenu, ...parentMenus]);
            }

            setHighlightKey(newKey);
            setHighlightType("CREATE");

            message.success("Thêm menu thành công");
        } else {
            const target = selectedChild || selectedParent;
            await updateMenuStatus({
                Code: target!.key,
                Name:target!.label,
                isUsing: values.isActive,
            });

            updateLocalMenu(target!.key, values.isActive, "MODIFY");
            message.success("Cập nhật menu thành công");
        }

        setModalOpen(false);
    };

    /* ======================
       RENDER
    ====================== */
    return (
        <>
            <Title level={4}>Quản lý Menu hệ thống</Title>

            <Row style={{ marginBottom: 16 }}>
                <Col span={6}>
                    <Select
                        placeholder="Chọn hệ thống"
                        style={{ width: "100%" }}
                        value={systemCode}
                        onChange={setSystemCode}
                        options={systems.map((s) => ({
                            value: s.code,
                            label: s.name,
                        }))}
                    />
                </Col>
            </Row>

            <Row gutter={16}>
                <Col span={10}>
                    <Table
                        rowKey="key"
                        size="small"
                        loading={loading}
                        dataSource={parentMenus}
                        expandable={{ showExpandColumn: false }}
                        rowClassName={(r) =>
                            r.key === highlightKey
                                ? highlightType === "CREATE"
                                    ? "row-created"
                                    : "row-modified"
                                : r.key === selectedParent?.key
                                    ? "row-selected"
                                    : ""
                        }
                        onRow={(r) => ({
                            onClick: () => {
                                setSelectedParent(r);
                                setSelectedChild(null);
                                setChildMenus(r.children || []);
                            },
                        })}
                        columns={[
                            { title: "Menu cha", dataIndex: "label" },
                            {
                                title: "Trạng thái",
                                width: 100,
                                render: (_, r) => (r.isUsing ? "Đang dùng" : "Ngừng"),
                            },
                        ]}
                    />
                </Col>

                <Col span={14}>
                    <Table
                        rowKey="key"
                        size="small"
                        dataSource={childMenus}
                        rowClassName={(r) =>
                            r.key === highlightKey
                                ? highlightType === "CREATE"
                                    ? "row-created"
                                    : "row-modified"
                                : r.key === selectedChild?.key
                                    ? "row-selected"
                                    : ""
                        }
                        onRow={(r) => ({
                            onClick: () => setSelectedChild(r),
                        })}
                        columns={[
                            { title: "Menu con", dataIndex: "label" },
                            {
                                title: "Trạng thái",
                                width: 100,
                                render: (_, r) => (r.isUsing ? "Đang dùng" : "Ngừng"),
                            },
                        ]}
                    />
                </Col>
            </Row>

            <Modal
                open={modalOpen}
                title={mode === "CREATE" ? "Thêm menu" : "Sửa menu"}
                onOk={onSave}
                onCancel={() => setModalOpen(false)}
                destroyOnClose
            >
                <Form form={form} layout="vertical">
                    <Form.Item
                        label="Tên menu"
                        name="name"
                        rules={[{ required: true, message: "Nhập tên menu" }]}
                    >
                        <Input />
                    </Form.Item>

                    {mode === "EDIT" && (
                        <Form.Item
                            label="Trạng thái"
                            name="isActive"
                            valuePropName="checked"
                        >
                            <Switch />
                        </Form.Item>
                    )}
                </Form>
            </Modal>
        </>
    );
};

export default SystemMenuPage;
