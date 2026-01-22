import {
    Table,
    Typography,
    Row,
    Col,
    Input,
    Modal,
    Form,
    Select,
    Tag,
    message,
} from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../stores/ui-command.store";
import {
    getAssetMiddleTypes,
    createAssetMiddleType,
    updateAssetMiddleType,
    toggleAssetMiddleType,
} from "../../api/asset-middle-type.api";
import { useAuthStore } from "../../auth/auth.store";

const { Title } = Typography;
const PAGE_SIZE = 10;

/* ================= CONST ================= */

const MACHINE_TYPE = [
    { value: "1", label: "Máy móc" },
    { value: "2", label: "Công dụng cụ" },
];

const INVENTORY_CHECK_TYPE = [
    { value: "1", label: "Kiểm số lượng" },
    { value: "2", label: "Kiểm quét" },
];

interface Props {
    bigType?: any;
    activeTab: "BIG" | "MIDDLE" | "SMALL";
    setActiveTab: (tab: "BIG" | "MIDDLE" | "SMALL") => void;
    onSelectMiddleType?: (row: any) => void;
}

const AssetMiddleTypeTab = ({ bigType, activeTab, setActiveTab, onSelectMiddleType }: Props) => {
    /* ================= STORE ================= */

    const command = useUiCommandStore((s) => s.command);
    const clearCommand = useUiCommandStore((s) => s.clear);
    const username = useAuthStore((s) => s.username);

    /* ================= STATE ================= */

    //const [filters, setFilters] = useState<{ keyword?: string }>({});
    const [filters, setFilters] = useState<{
        keyword?: string;
        machineType?: string;
        inventoryCheckType?: string;
    }>({});

    const [data, setData] = useState<any[]>([]);
    const [loading, setLoading] = useState(false);

    const [page, setPage] = useState(1);
    const [total, setTotal] = useState(0);

    const [selectedRow, setSelectedRow] = useState<any>();
    const [modalOpen, setModalOpen] = useState(false);
    const [mode, setMode] = useState<"CREATE" | "EDIT">("CREATE");

    const [form] = Form.useForm();

    

    /* ================= COMMAND HANDLER ================= */

    useEffect(() => {
        if (!command) return;
        if (activeTab !== "MIDDLE") return;
        switch (command) {
            case "QUERY":
                handleQuery(1);
                break;

            case "INSERT":
                setMode("CREATE");
                setSelectedRow(undefined);
                form.resetFields();
                setModalOpen(true);
                break;

            case "MODIFY":
                if (!selectedRow) {
                    message.warning("Vui lòng chọn dữ liệu");
                    break;
                }

                setMode("EDIT");
                form.setFieldsValue({
                    nameEN: selectedRow.middleTypeNameEN,
                    nameVN: selectedRow.middleTypeNameVN,
                    machine_Type: selectedRow.machine_Type,
                    inventory_Check_Type: selectedRow.inventory_Check_Type,
                });
                setModalOpen(true);
                break;

            case "DELETE":
                if (!selectedRow) {
                    message.warning("Vui lòng chọn dữ liệu");
                    break;
                }

                Modal.confirm({
                    title: "Ẩn / Hiện dữ liệu?",
                    onOk: async () => {
                        await toggleAssetMiddleType(
                            selectedRow.middleTypeId,
                            username!
                        );
                        message.success("Cập nhật trạng thái thành công");
                        handleQuery(page);
                    },
                });
                break;
        }

        clearCommand();
    }, [command]);

    /* ================= QUERY ================= */

    const handleQuery = async (p = page) => {
        setLoading(true);
        try {
            //   const res = await getAssetMiddleTypes({
            //     keyword: filters.keyword,
            //     bigTypeId: bigType.bigTypeId,
            //     page: p,
            //     pageSize: PAGE_SIZE,
            //   });
            const res = await getAssetMiddleTypes({
                keyword: filters.keyword,
                machineType: filters.machineType,
                inventoryCheckType: filters.inventoryCheckType,
                bigTypeId: bigType.bigTypeId,
                page: p,
                pageSize: PAGE_SIZE,
            });

            console.log(res.items);
            setData(res.items);
            setTotal(res.total);
            setPage(p);
        } finally {
            setLoading(false);
        }
    };

    /* ================= SAVE ================= */

    const onSave = async () => {
        const values = await form.validateFields();

        const payload = {
            ...values,
            bigTypeId: bigType.bigTypeId,
        };

        if (mode === "CREATE") {
            await createAssetMiddleType(payload, username!);
            message.success("Thêm loại vừa thành công");
            handleQuery(1);
        } else {
            await updateAssetMiddleType(
                selectedRow.middleTypeId,
                payload,
                username!
            );
            message.success("Cập nhật thành công");
            handleQuery(page);
        }

        setModalOpen(false);
    };
    /* ================= GUARD ================= */

    if (!bigType) {
        return (
            <div style={{ padding: 24 }}>
                <Tag color="orange">
                    Vui lòng chọn <b>Loại lớn</b> trước
                </Tag>
            </div>
        );
    }
    /* ================= RENDER ================= */

    return (
        <>
            <Title level={4}>
                Phân loại tài sản – Loại vừa ({bigType.bigTypeNameVN})
            </Title>

            {/* <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={6}>
          <Input
            placeholder="Từ khóa"
            allowClear
            onChange={(e) =>
              setFilters({ keyword: e.target.value })
            }
          />
        </Col>
      </Row> */}
            <Row gutter={16} style={{ marginBottom: 16 }}>
                <Col span={6}>
                    <Input
                        placeholder="Từ khóa (EN / VN)"
                        allowClear
                        onChange={(e) =>
                            setFilters((p) => ({ ...p, keyword: e.target.value }))
                        }
                    />
                </Col>

                <Col span={5}>
                    <Select
                        placeholder="Loại thiết bị"
                        allowClear
                        options={MACHINE_TYPE}
                        onChange={(v) =>
                            setFilters((p) => ({ ...p, machineType: v }))
                        }
                    />
                </Col>

                <Col span={5}>
                    <Select
                        placeholder="Hình thức kiểm kê"
                        allowClear
                        options={INVENTORY_CHECK_TYPE}
                        onChange={(v) =>
                            setFilters((p) => ({ ...p, inventoryCheckType: v }))
                        }
                    />
                </Col>
            </Row>


            <Table
                rowKey="middleTypeId"
                loading={loading}
                dataSource={data}
                pagination={{
                    current: page,
                    pageSize: PAGE_SIZE,
                    total,
                    onChange: handleQuery,
                }}
                onRow={(record) => ({
                    onClick: () => {
                        setSelectedRow(record);
                        setActiveTab("MIDDLE");
                        onSelectMiddleType?.(record);
                    }
                })}
                rowClassName={(record) =>
                    record.middleTypeId === selectedRow?.middleTypeId
                        ? "ant-table-row-selected"
                        : ""
                }
                columns={[
                    {
                        title: "STT",
                        width: 60,
                        render: (_: any, __: any, index: number) =>
                            (page - 1) * PAGE_SIZE + index + 1,
                    },
                    {
                        title: "Tên EN",
                        dataIndex: "nameEN",
                    },
                    {
                        title: "Tên VN",
                        dataIndex: "nameVN",
                    },
                    {
                        title: "Loại thiết bị",
                        dataIndex: "machine_Type",
                        render: (v) =>
                            v === "1" ? (
                                <Tag color="blue">Máy móc</Tag>
                            ) : (
                                <Tag color="green">Công dụng cụ</Tag>
                            ),
                    },
                    {
                        title: "Kiểm kê",
                        dataIndex: "inventory_Check_Type",
                        render: (v) =>
                            v === "1" ? (
                                <Tag color="orange">Kiểm số lượng</Tag>
                            ) : (
                                <Tag color="purple">Kiểm quét</Tag>
                            ),
                    },
                    {
                        title: "Trạng thái",
                        dataIndex: "isActive",
                        render: (v) =>
                            v ? (
                                <Tag color="green">Hoạt động</Tag>
                            ) : (
                                <Tag color="red">Ẩn</Tag>
                            ),
                    },
                ]}

            />

            {/* ================= MODAL ================= */}

            <Modal
                open={modalOpen}
                title={mode === "CREATE" ? "Thêm loại vừa" : "Sửa loại vừa"}
                onOk={onSave}
                onCancel={() => setModalOpen(false)}
                destroyOnClose
            >
                <Form form={form} layout="vertical">
                    <Form.Item
                        name="nameEN"
                        label="Tên EN"
                        rules={[{ required: true }]}
                    >
                        <Input />
                    </Form.Item>

                    <Form.Item
                        name="nameVN"
                        label="Tên VN"
                        rules={[{ required: true }]}
                    >
                        <Input />
                    </Form.Item>

                    <Form.Item
                        name="machine_Type"
                        label="Loại thiết bị"
                        rules={[{ required: true }]}
                    >
                        <Select options={MACHINE_TYPE} />
                    </Form.Item>

                    <Form.Item
                        name="inventory_Check_Type"
                        label="Hình thức kiểm kê"
                        rules={[{ required: true }]}
                    >
                        <Select options={INVENTORY_CHECK_TYPE} />
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
};

export default AssetMiddleTypeTab;
