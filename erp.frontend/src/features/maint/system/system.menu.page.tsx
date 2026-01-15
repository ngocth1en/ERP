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
    orderNo?: number;
    children?: MenuDto[];
}

/* ======================
   COMPONENT
====================== */
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
       HELPERS
    ====================== */
    const extractNumberFromCode = (code: string) => {
        const match = code.match(/(\d+)$/);
        return match ? parseInt(match[1], 10) : 0;
    };

    const getNextOrderNo = () => {
        const list = selectedParent
            ? selectedParent.children || []
            : parentMenus;

        if (list.length === 0) return 1;

        const maxNo = Math.max(
            ...list.map((m: MenuDto) => extractNumberFromCode(m.key))
        );


        return maxNo + 1;
    };


    const generateMenuCode = (systemCode: string, orderNo: number) =>
        `${systemCode}${orderNo.toString().padStart(3, "0")}`;

    /* ======================
       HANDLERS
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
        if (!systemCode) {
            message.warning("Vui lòng chọn hệ thống");
            return;
        }

        if (selectedParent && !selectedParent.isUsing) {
            message.warning("Menu cha đang ngừng");
            return;
        }

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
            Name: target.label,
            isUsing: false,
        });

        updateLocalMenu(target.key, false);
        message.success("Đã ngừng menu");
    };

    /* ======================
       LOCAL UPDATE
    ====================== */
    const updateLocalMenu = (key: string, isUsing: boolean) => {
        setHighlightKey(key);
        setHighlightType("MODIFY");

        setParentMenus((prev) => {
            const copy = [...prev];

            const pIndex = copy.findIndex((p) => p.key === key);
            if (pIndex !== -1) {
                copy[pIndex] = { ...copy[pIndex], isUsing };
                return [...copy];
            }

            for (const p of copy) {
                const cIndex = p.children?.findIndex((c) => c.key === key) ?? -1;
                if (cIndex !== -1 && p.children) {
                    p.children[cIndex] = {
                        ...p.children[cIndex],
                        isUsing,
                    };
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
            const orderNo = getNextOrderNo();
            const code = generateMenuCode(systemCode!, orderNo);

            const newMenu: MenuDto = {
                key: code,
                label: values.name,
                isUsing: true,
                orderNo,
            };

            await createSystemMenu({
                code,
                systemCode: systemCode!,
                parentCode: selectedParent?.key,
                name: values.name,
                route: values.route,
                orderNo,
            });

            if (selectedParent) {
                selectedParent.children = [
                    newMenu,
                    ...(selectedParent.children || []),
                ];
                setChildMenus([...selectedParent.children]);
            } else {
                setParentMenus([newMenu, ...parentMenus]);
            }

            setHighlightKey(code);
            setHighlightType("CREATE");
            message.success("Thêm menu thành công");
        } else {
            const target = selectedChild || selectedParent;

            await updateMenuStatus({
                Code: target!.key,
                Name: values.name,
                isUsing: values.isActive,
            });

            updateLocalMenu(target!.key, values.isActive);
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
                                render: (_, r) =>
                                    r.isUsing ? "Đang dùng" : "Ngừng",
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
                                render: (_, r) =>
                                    r.isUsing ? "Đang dùng" : "Ngừng",
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

                    {mode === "CREATE" && (
                        <Form.Item
                            label="Route"
                            name="route"
                            rules={[
                                { required: true, message: "Nhập route" },
                            ]}
                        >
                            <Input placeholder="/asset/maintenance" />
                        </Form.Item>
                    )}

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
