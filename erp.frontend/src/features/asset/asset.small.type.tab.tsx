import {
    Table,
    Typography,
    Row,
    Col,
    Input,
    Modal,
    Form,
    Tag,
    message,
} from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../stores/ui-command.store";
import {
    getAssetSmallTypes,
    createAssetSmallType,
    updateAssetSmallType,
    toggleAssetSmallType,
} from "../../api/asset-small-type.api";
import { useAuthStore } from "../../auth/auth.store";

const { Title } = Typography;
const PAGE_SIZE = 10;

interface Props {
    middleType?: any;
    activeTab: "BIG" | "MIDDLE" | "SMALL";
    setActiveTab: (tab: "BIG" | "MIDDLE" | "SMALL") => void;
}

const AssetSmallTypeTab = ({ middleType, activeTab, setActiveTab }: Props) => {
    /* ================= STORE ================= */

    const command = useUiCommandStore((s) => s.command);
    const clearCommand = useUiCommandStore((s) => s.clear);
    const username = useAuthStore((s) => s.username);

    /* ================= STATE ================= */

    const [filters, setFilters] = useState<{ keyword?: string }>({});
    const [data, setData] = useState<any[]>([]);
    const [loading, setLoading] = useState(false);

    const [page, setPage] = useState(1);
    const [total, setTotal] = useState(0);

    const [selectedRow, setSelectedRow] = useState<any>();
    const [modalOpen, setModalOpen] = useState(false);
    const [mode, setMode] = useState<"CREATE" | "EDIT">("CREATE");

    const [form] = Form.useForm();



    /* ================= COMMAND ================= */

    useEffect(() => {
        if (!command || activeTab !== "SMALL") return;

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
                    nameEN: selectedRow.nameEN,
                    nameVN: selectedRow.nameVN,
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
                        await toggleAssetSmallType(
                            selectedRow.smallTypeId,
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
            const res = await getAssetSmallTypes({
                keyword: filters.keyword,
                middleTypeId: middleType.middleTypeId,
                page: p,
                pageSize: PAGE_SIZE,
            });

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
            middleTypeId: middleType.middleTypeId,
        };

        if (mode === "CREATE") {
            await createAssetSmallType(payload, username!);
            message.success("Thêm loại nhỏ thành công");
            handleQuery(1);
        } else {
            await updateAssetSmallType(
                selectedRow.smallTypeId,
                payload,
                username!
            );
            message.success("Cập nhật thành công");
            handleQuery(page);
        }

        setModalOpen(false);
    };
    /* ================= GUARD ================= */

    if (!middleType) {
        return (
            <div style={{ padding: 24 }}>
                <Tag color="orange">
                    Vui lòng chọn <b>Loại vừa</b> trước
                </Tag>
            </div>
        );
    }
    /* ================= RENDER ================= */

    return (
        <>
            <Title level={4}>
                Phân loại tài sản – Loại nhỏ ({middleType.middleTypeNameVN})
            </Title>

            <Row gutter={16} style={{ marginBottom: 16 }}>
                <Col span={6}>
                    <Input
                        placeholder="Từ khóa (EN / VN)"
                        allowClear
                        onChange={(e) =>
                            setFilters({ keyword: e.target.value })
                        }
                    />
                </Col>
            </Row>

            <Table
                rowKey="smallTypeId"
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
                        setSelectedRow(record),
                            setActiveTab("SMALL");
                    }
                })}
                rowClassName={(record) =>
                    record.smallTypeId === selectedRow?.smallTypeId
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

            <Modal
                open={modalOpen}
                title={mode === "CREATE" ? "Thêm loại nhỏ" : "Sửa loại nhỏ"}
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
                </Form>
            </Modal>
        </>
    );
};

export default AssetSmallTypeTab;
