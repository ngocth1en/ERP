import React, { useEffect, useState, useCallback, useRef } from "react";
import {
    Button, Card, Col, Divider, Form, Grid, Input, InputNumber, Modal,
    Row, Select, Table, Tag, Tabs, Typography, message, Spin,
} from "antd";
import { SearchOutlined } from "@ant-design/icons";
import type { ColumnsType } from "antd/es/table";
import { useUiCommandStore } from "../../stores/ui-command.store";
import { getDepartmentOptions, type DepartmentOptionDto } from "../../api/department.api";
import {
    createRepair, updateRepair, searchRepairs, getRepair,
    startRepair, completeRepair, cancelRepair, searchScrappedAssets,
    RepairStatus, REPAIR_STATUS_CONFIG,
    type RepairOrderDto, type RepairOrderListDto,
    type ScrappedAssetCandidateDto,
    type CreateRepairDto, type UpdateRepairDto,
} from "../../api/repair.api";

const { Text, Title } = Typography;
const { useBreakpoint } = Grid;
const PAGE_SIZE = 20;
const MODAL_ASSET_PAGE_SIZE = 10;

const AssetRepairPage: React.FC = () => {
    const screens = useBreakpoint();
    const isMobile = !screens.md;

    const command = useUiCommandStore((s) => s.command);
    const clearCommand = useUiCommandStore((s) => s.clear);

    const [departments, setDepartments] = useState<DepartmentOptionDto[]>([]);
    const [activeTab, setActiveTab] = useState("1");

    const [orders, setOrders] = useState<RepairOrderListDto[]>([]);
    const [total, setTotal] = useState(0);
    const [pageIndex, setPageIndex] = useState(1);
    const [loadingList, setLoadingList] = useState(false);

    const [filterDept, setFilterDept] = useState<string | undefined>();
    const [filterStatus, setFilterStatus] = useState<RepairStatus | undefined>();

    const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
    const [selectedOrder, setSelectedOrder] = useState<RepairOrderDto | null>(null);
    const [loadingDetail, setLoadingDetail] = useState(false);

    const selectedOrderRef = useRef<RepairOrderDto | null>(null);
    const openEditModalRef = useRef<(order: RepairOrderDto) => void>(() => { });
    const openCreateModalRef = useRef<() => void>(() => { });
    const loadOrdersRef = useRef<(page?: number) => void>(() => { });
    const cancelOpenRef = useRef<(v: boolean) => void>(() => { });

    /* ── modal tạo ── */
    const [createOpen, setCreateOpen] = useState(false);
    const [saving, setSaving] = useState(false);
    const [formCreate] = Form.useForm();
    const [scrappedAssets, setScrappedAssets] = useState<ScrappedAssetCandidateDto[]>([]);
    const [scrappedTotal, setScrappedTotal] = useState(0);
    const [scrappedPage, setScrappedPage] = useState(1);
    const [loadingScrapped, setLoadingScrapped] = useState(false);
    const [scrappedKeyword, setScrappedKeyword] = useState("");
    const [selectedAsset, setSelectedAsset] = useState<ScrappedAssetCandidateDto | null>(null);

    /* ── modal sửa ── */
    const [editOpen, setEditOpen] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [savingEdit, setSavingEdit] = useState(false);
    const [formEdit] = Form.useForm<UpdateRepairDto>();

    /* ── modal action ── */
    const [startOpen, setStartOpen] = useState(false);
    const [completeOpen, setCompleteOpen] = useState(false);
    const [cancelOpen, setCancelOpen] = useState(false);

    /* ════════════════════════════════════════════════════════════
       PERMISSION HELPERS
    ════════════════════════════════════════════════════════════ */
    const normalizeStatus = (status: any): RepairStatus | undefined => {
        if (typeof status === "number") return status as RepairStatus;
        if (typeof status === "string") return RepairStatus[status as keyof typeof RepairStatus] as unknown as RepairStatus;
        return undefined;
    };

    const checkCanEdit = (order: RepairOrderDto | null) => {
        const s = normalizeStatus(order?.status);
        return s === RepairStatus.Draft || s === RepairStatus.Pending;
    };
    const checkCanStart = (order: RepairOrderDto | null) => normalizeStatus(order?.status) === RepairStatus.Pending;
    const checkCanComplete = (order: RepairOrderDto | null) => normalizeStatus(order?.status) === RepairStatus.InProgress;
    const checkCanCancel = (order: RepairOrderDto | null) => {
        const s = normalizeStatus(order?.status);
        return s === RepairStatus.Draft || s === RepairStatus.Pending || s === RepairStatus.InProgress;
    };

    /* ════════════════════════════════════════════════════════════
       LOAD LOOKUP
    ════════════════════════════════════════════════════════════ */
    useEffect(() => {
        getDepartmentOptions().then((r) => setDepartments(r.data))
            .catch(() => message.error("Không tải được dữ liệu lookup"));
    }, []);

    const getDeptName = (id?: string) => departments.find((d) => d.id === id)?.name ?? id ?? "—";

    /* ════════════════════════════════════════════════════════════
       LOAD ORDERS
    ════════════════════════════════════════════════════════════ */
    const loadOrders = useCallback(async (page = 1) => {
        setLoadingList(true);
        try {
            const res = await searchRepairs({
                departmentId: filterDept, status: filterStatus,
                pageIndex: page, pageSize: PAGE_SIZE,
            });
            setOrders(res.items); setTotal(res.total); setPageIndex(page);
        } catch { message.error("Không tải được danh sách đơn"); }
        finally { setLoadingList(false); }
    }, [filterDept, filterStatus]);

    useEffect(() => { loadOrders(1); }, [loadOrders]);

    /* ════════════════════════════════════════════════════════════
       LOAD / REFRESH DETAIL
    ════════════════════════════════════════════════════════════ */
    const loadDetail = async (id: string) => {
        setLoadingDetail(true);
        try {
            const res = await getRepair(id);
            setSelectedOrder(res); selectedOrderRef.current = res; setSelectedOrderId(id);
        } catch { message.error("Không tải được chi tiết đơn"); }
        finally { setLoadingDetail(false); }
    };

    const refreshDetail = async () => {
        if (!selectedOrderId) return;
        try {
            const res = await getRepair(selectedOrderId);
            setSelectedOrder(res); selectedOrderRef.current = res;
        } catch { /* silent */ }
    };

    /* ════════════════════════════════════════════════════════════
       MODAL TẠO
    ════════════════════════════════════════════════════════════ */
        const [scrappedDept, setScrappedDept] = useState<string | undefined>();

    const loadScrappedAssets = useCallback(async (deptId: string | undefined, keyword: string, page: number) => {
        setLoadingScrapped(true);
        try {
            const res = await searchScrappedAssets({
                departmentId: deptId,
                assetNo: keyword || undefined,
                pageIndex: page,
                pageSize: MODAL_ASSET_PAGE_SIZE,
            });
            setScrappedAssets(res.items); setScrappedTotal(res.total); setScrappedPage(page);
        } catch { message.error("Không tải được danh sách tài sản đã báo phế"); }
        finally { setLoadingScrapped(false); }
    }, []);

    const openCreateModal = () => {
        formCreate.resetFields();
        setSelectedAsset(null);
        setScrappedDept(undefined);
        setScrappedKeyword("");
        setScrappedAssets([]); setScrappedTotal(0); setScrappedPage(1);
        setCreateOpen(true);
        loadScrappedAssets(undefined, "", 1);
    };

    const handleCreateSubmit = async () => {
        if (!selectedAsset) { message.warning("Chọn 1 tài sản đã báo phế để tạo đơn sửa chữa"); return; }
        try {
            const values = await formCreate.validateFields();
            setSaving(true);
            const created = await createRepair({
                assetNo: selectedAsset.assetNo,
                scrapId: selectedAsset.scrapId,
                repairVendor: values.repairVendor,
                reason: values.reason,
                cost: values.cost,
                note: values.note,
            } as CreateRepairDto);
            message.success("Tạo đơn sửa chữa thành công");
            setCreateOpen(false);
            await loadOrders(1);
            await loadDetail(created.id);
        } catch (e: any) {
            message.error(e?.response?.data?.message ?? "Lưu thất bại");
        } finally { setSaving(false); }
    };

    /* ════════════════════════════════════════════════════════════
       MODAL SỬA
    ════════════════════════════════════════════════════════════ */
    const openEditModal = (order: RepairOrderDto) => {
        setEditingId(order.id);
        formEdit.setFieldsValue({
            repairVendor: order.repairVendor, reason: order.reason,
            cost: order.cost, note: order.note,
        });
        setEditOpen(true);
    };

    const handleEditSubmit = async (values: any) => {
        if (!editingId) return;
        setSavingEdit(true);
        try {
            await updateRepair(editingId, values as UpdateRepairDto);
            message.success("Cập nhật đơn thành công");
            setEditOpen(false);
            await refreshDetail();
        } catch (e: any) { message.error(e?.response?.data?.message ?? "Lưu thất bại"); }
        finally { setSavingEdit(false); }
    };

    /* ════════════════════════════════════════════════════════════
       ACTIONS
    ════════════════════════════════════════════════════════════ */
    const handleStart = async () => {
        if (!selectedOrder) return;
        try {
            await startRepair(selectedOrder.id);
            message.success("Đã bắt đầu sửa chữa");
            setStartOpen(false);
            await Promise.all([loadOrders(pageIndex), refreshDetail()]);
        } catch (e: any) { message.error(e?.response?.data?.message ?? "Thao tác thất bại"); }
    };

    const handleComplete = async () => {
        if (!selectedOrder) return;
        try {
            await completeRepair(selectedOrder.id);
            message.success("Hoàn thành sửa chữa, tài sản đã được phục hồi");
            setCompleteOpen(false);
            await Promise.all([loadOrders(pageIndex), refreshDetail()]);
        } catch (e: any) { message.error(e?.response?.data?.message ?? "Thao tác thất bại"); }
    };

    const handleCancel = async () => {
        if (!selectedOrder) return;
        try {
            await cancelRepair(selectedOrder.id);
            message.success("Đã hủy đơn sửa chữa");
            setCancelOpen(false);
            await Promise.all([loadOrders(pageIndex), refreshDetail()]);
        } catch (e: any) { message.error(e?.response?.data?.message ?? "Hủy thất bại"); }
    };

    /* ════════════════════════════════════════════════════════════
       REFS & TOOLBAR
    ════════════════════════════════════════════════════════════ */
    useEffect(() => {
        selectedOrderRef.current = selectedOrder;
        openEditModalRef.current = openEditModal;
        openCreateModalRef.current = openCreateModal;
        loadOrdersRef.current = loadOrders;
        cancelOpenRef.current = setCancelOpen;
    });

    useEffect(() => {
        if (!command) return;
        const order = selectedOrderRef.current;
        switch (command) {
            case "QUERY": loadOrdersRef.current(1); break;
            case "INSERT": openCreateModalRef.current(); break;
            case "MODIFY":
                if (checkCanEdit(order)) openEditModalRef.current(order!);
                else message.warning("Chọn đơn Nháp hoặc Chờ duyệt để sửa");
                break;
            case "CONFIRM":
                if (checkCanStart(order)) setStartOpen(true);
                else if (checkCanComplete(order)) setCompleteOpen(true);
                else message.warning("Chọn đơn Chờ duyệt hoặc Đang sửa chữa");
                break;
            case "DELETE":
                if (checkCanCancel(order)) cancelOpenRef.current(true);
                else message.warning("Chọn đơn chưa hoàn thành để hủy");
                break;
            default: break;
        }
        clearCommand();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [command]);

    /* ════════════════════════════════════════════════════════════
       COLUMNS
    ════════════════════════════════════════════════════════════ */
    const deptOptions = departments.map((d) => ({ value: d.id, label: d.name }));
    const statusOptions = Object.entries(REPAIR_STATUS_CONFIG).map(([k, v]) => ({ value: Number(k), label: v.label }));
    const filterOption = (input: string, option?: { label: string; value: string }) =>
        (option?.label ?? "").toLowerCase().includes(input.toLowerCase());

    const orderColumns: ColumnsType<RepairOrderListDto> = [
        { title: "Mã đơn", dataIndex: "id", key: "id", width: 160, fixed: "left" as const, render: (id) => <a onClick={() => loadDetail(id)}>{id}</a> },
        { title: "Mã tài sản", dataIndex: "assetNo", key: "assetNo", width: 140 },
        { title: "Tên tài sản", dataIndex: "assetName", key: "assetName", ellipsis: true },
        { title: "Đơn vị", dataIndex: "departmentName", key: "dept", ellipsis: true },
        {
            title: "Trạng thái", dataIndex: "status", key: "status", width: 140,
            render: (s: RepairStatus) => {
                const cfg = REPAIR_STATUS_CONFIG[s];
                return <Tag color={cfg?.color ?? "default"}>{cfg?.label ?? `(${s})`}</Tag>;
            },
        },
        { title: "Người tạo", dataIndex: "createdByUserName", key: "creator", width: 130 },
        { title: "Ngày tạo", dataIndex: "createdAt", key: "createdAt", width: 110, render: (v: string) => new Date(v).toLocaleDateString("vi-VN") },
    ];

    const scrappedColumns: ColumnsType<ScrappedAssetCandidateDto> = [
        { title: "Mã tài sản", dataIndex: "assetNo", key: "assetNo", width: 140 },
        { title: "Tên tài sản", dataIndex: "assetName", key: "assetName", ellipsis: true },
        { title: "Model", dataIndex: "modelName", key: "model", width: 160, ellipsis: true },
        { title: "Phiếu báo phế", dataIndex: "scrapId", key: "scrapId", width: 140 },
        { title: "Ngày báo phế", dataIndex: "scrapConfirmedAt", key: "scrapDate", width: 120, render: (v: string) => new Date(v).toLocaleDateString("vi-VN") },
    ];

    const tabItems = [
        {
            key: "1", label: "Danh sách đơn",
            children: (
                <>
                    <Card size="small" style={{ marginBottom: 12 }}>
                        <Row gutter={[8, 8]}>
                            <Col xs={24} sm={12} md={8}>
                                <Select allowClear placeholder="Đơn vị" style={{ width: "100%" }}
                                    options={deptOptions} showSearch filterOption={filterOption}
                                    onChange={(v) => setFilterDept(v)} />
                            </Col>
                            <Col xs={24} sm={12} md={6}>
                                <Select allowClear placeholder="Trạng thái" style={{ width: "100%" }}
                                    options={statusOptions} onChange={(v) => setFilterStatus(v)} />
                            </Col>
                        </Row>
                    </Card>
                    <Table rowKey="id" dataSource={orders} columns={orderColumns} loading={loadingList}
                        size="small" scroll={{ x: "max-content" }}
                        onRow={(record) => ({
                            onClick: () => loadDetail(record.id),
                            style: { cursor: "pointer", background: record.id === selectedOrderId ? "#e6f4ff" : undefined },
                        })}
                        pagination={{ total, pageSize: PAGE_SIZE, current: pageIndex, onChange: (p) => loadOrders(p), showSizeChanger: false, showTotal: (t) => `Tổng ${t} đơn` }} />
                </>
            ),
        },
        {
            key: "2", label: "Chi tiết đơn",
            children: (
                <>
                    {!selectedOrderId && <div style={{ padding: 24, textAlign: "center" }}><Text type="secondary">Chọn một đơn từ tab "Danh sách đơn" để xem chi tiết.</Text></div>}
                    {loadingDetail && selectedOrderId && <div style={{ textAlign: "center", padding: 24 }}><Spin /></div>}
                    {selectedOrder && !loadingDetail && (
                        <Card size="small">
                            <Row gutter={[16, 8]}>
                                <Col xs={12} md={4}><Text type="secondary">Mã đơn</Text><br /><Text strong>{selectedOrder.id}</Text></Col>
                                <Col xs={12} md={4}>
                                    <Text type="secondary">Trạng thái</Text><br />
                                    <Tag color={REPAIR_STATUS_CONFIG[selectedOrder.status]?.color ?? "default"}>
                                        {REPAIR_STATUS_CONFIG[selectedOrder.status]?.label ?? `(${selectedOrder.status})`}
                                    </Tag>
                                </Col>
                                <Col xs={12} md={4}><Text type="secondary">Mã tài sản</Text><br /><Text>{selectedOrder.assetNo}</Text></Col>
                                <Col xs={12} md={6}><Text type="secondary">Tên tài sản</Text><br /><Text>{selectedOrder.assetName}</Text></Col>
                                <Col xs={12} md={4}><Text type="secondary">Đơn vị</Text><br /><Text>{selectedOrder.departmentName}</Text></Col>
                                {selectedOrder.scrapId && <Col xs={12} md={4}><Text type="secondary">Phiếu báo phế</Text><br /><Text>{selectedOrder.scrapId}</Text></Col>}
                                {selectedOrder.repairVendor && <Col xs={12} md={6}><Text type="secondary">Đơn vị sửa chữa</Text><br /><Text>{selectedOrder.repairVendor}</Text></Col>}
                                {selectedOrder.cost != null && <Col xs={12} md={4}><Text type="secondary">Chi phí</Text><br /><Text>{selectedOrder.cost.toLocaleString("vi-VN")} đ</Text></Col>}
                                {selectedOrder.completedAt && <Col xs={12} md={4}><Text type="secondary">Ngày hoàn thành</Text><br /><Text>{new Date(selectedOrder.completedAt).toLocaleDateString("vi-VN")}</Text></Col>}
                                <Col xs={24} md={8}><Text type="secondary">Lý do</Text><br /><Text>{selectedOrder.reason ?? "—"}</Text></Col>
                                <Col xs={24} md={8}><Text type="secondary">Ghi chú</Text><br /><Text>{selectedOrder.note ?? "—"}</Text></Col>
                                <Col xs={12} md={4}><Text type="secondary">Người tạo</Text><br /><Text>{selectedOrder.createdByUserName}</Text></Col>
                                {selectedOrder.completedByUserName && <Col xs={12} md={4}><Text type="secondary">Người hoàn thành</Text><br /><Text>{selectedOrder.completedByUserName}</Text></Col>}
                            </Row>
                        </Card>
                    )}
                </>
            ),
        },
    ];

    return (
        <div style={{ padding: isMobile ? 8 : 16 }}>
            <Title level={4} style={{ marginBottom: 16 }}>🔧 Sửa chữa tài sản</Title>
            <Tabs activeKey={activeTab} onChange={setActiveTab} items={tabItems} size={isMobile ? "small" : "middle"} />

            {/* ══ MODAL TẠO ══ */}
            <Modal title="Tạo đơn sửa chữa" open={createOpen} onCancel={() => setCreateOpen(false)}
                onOk={handleCreateSubmit} okText="Tạo đơn" okButtonProps={{ disabled: !selectedAsset }}
                confirmLoading={saving} width={isMobile ? "98vw" : 900} destroyOnClose>
                <Form form={formCreate} layout="vertical">
                    <Row gutter={12}>
                        <Col xs={24} sm={12}>
                            <Form.Item name="repairVendor" label="Đơn vị / nhà cung cấp sửa chữa">
                                <Input placeholder="VD: Công ty ABC" />
                            </Form.Item>
                        </Col>
                        <Col xs={24} sm={12}>
                            <Form.Item name="cost" label="Chi phí dự kiến (VNĐ)">
                                <InputNumber style={{ width: "100%" }} min={0} formatter={(v) => `${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",")} />
                            </Form.Item>
                        </Col>
                        <Col xs={24}>
                            <Form.Item name="reason" label="Mô tả hư hỏng / lý do sửa chữa">
                                <Input.TextArea rows={2} />
                            </Form.Item>
                        </Col>
                        <Col xs={24}>
                            <Form.Item name="note" label="Ghi chú">
                                <Input.TextArea rows={2} />
                            </Form.Item>
                        </Col>
                    </Row>
                </Form>

                <Divider style={{ margin: "8px 0 12px" }}>
                    Chọn tài sản đã báo phế để sửa chữa
                    {selectedAsset && <Tag color="blue" style={{ marginLeft: 8 }}>{selectedAsset.assetNo}</Tag>}
                </Divider>

                                <Row gutter={[8, 8]} style={{ marginBottom: 8 }}>
                    <Col xs={24} sm={8}>
                        <Select allowClear placeholder="Lọc theo đơn vị" style={{ width: "100%" }}
                            options={deptOptions} showSearch filterOption={filterOption}
                            value={scrappedDept}
                            onChange={(v) => { setScrappedDept(v); setScrappedPage(1); loadScrappedAssets(v, scrappedKeyword, 1); }} />
                    </Col>
                    <Col xs={24} sm={10}>
                        <Input placeholder="Tìm theo mã tài sản" value={scrappedKeyword}
                            onChange={(e) => setScrappedKeyword(e.target.value)}
                            onPressEnter={() => { setScrappedPage(1); loadScrappedAssets(scrappedDept, scrappedKeyword, 1); }} allowClear />
                    </Col>
                    <Col xs={24} sm={6}>
                        <Button icon={<SearchOutlined />} block onClick={() => { setScrappedPage(1); loadScrappedAssets(scrappedDept, scrappedKeyword, 1); }}>Tìm</Button>
                    </Col>
                </Row>
                <Table rowKey="assetNo" dataSource={scrappedAssets} columns={scrappedColumns}
                    loading={loadingScrapped} size="small" scroll={{ x: 620 }}
                    rowSelection={{
                        type: "radio",
                        selectedRowKeys: selectedAsset ? [selectedAsset.assetNo] : [],
                        onChange: (_, rows) => setSelectedAsset(rows[0] ?? null),
                    }}
                    pagination={{ total: scrappedTotal, pageSize: MODAL_ASSET_PAGE_SIZE, current: scrappedPage, onChange: (p) => loadScrappedAssets(scrappedDept, scrappedKeyword, p), showSizeChanger: false, showTotal: (t) => `${t} tài sản` }} />
            </Modal>

            {/* ══ MODAL SỬA ══ */}
            <Modal title="Sửa đơn sửa chữa" open={editOpen} onCancel={() => setEditOpen(false)}
                onOk={() => formEdit.submit()} okText="Lưu" confirmLoading={savingEdit}
                width={isMobile ? "95vw" : 560} destroyOnClose>
                <Form form={formEdit} layout="vertical" onFinish={handleEditSubmit}>
                    <Form.Item name="repairVendor" label="Đơn vị / nhà cung cấp sửa chữa"><Input /></Form.Item>
                    <Form.Item name="cost" label="Chi phí (VNĐ)"><InputNumber style={{ width: "100%" }} min={0} /></Form.Item>
                    <Form.Item name="reason" label="Lý do"><Input.TextArea rows={2} /></Form.Item>
                    <Form.Item name="note" label="Ghi chú"><Input.TextArea rows={2} /></Form.Item>
                </Form>
            </Modal>

            {/* ══ MODAL BẮT ĐẦU ══ */}
            <Modal title="Bắt đầu sửa chữa?" open={startOpen} onOk={handleStart} onCancel={() => setStartOpen(false)} okText="Bắt đầu">
                <p>Đơn <strong>{selectedOrder?.id}</strong> sẽ chuyển sang trạng thái Đang sửa chữa.</p>
            </Modal>

            {/* ══ MODAL HOÀN THÀNH ══ */}
            <Modal title="Hoàn thành sửa chữa?" open={completeOpen} onOk={handleComplete} onCancel={() => setCompleteOpen(false)} okText="Hoàn thành">
                <p>Tài sản <strong>{selectedOrder?.assetNo}</strong> sẽ được phục hồi để sử dụng lại.</p>
            </Modal>

            {/* ══ MODAL HỦY ══ */}
            <Modal title="Hủy đơn sửa chữa?" open={cancelOpen} onOk={handleCancel} onCancel={() => setCancelOpen(false)} okText="Hủy đơn" okButtonProps={{ danger: true }}>
                <p>Đơn <strong>{selectedOrder?.id}</strong> sẽ bị hủy và không thể khôi phục.</p>
            </Modal>
        </div>
    );
};

export default AssetRepairPage;