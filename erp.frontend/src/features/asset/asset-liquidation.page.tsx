import React, { useEffect, useState, useCallback, useRef } from "react";
import {
    Button, Card, Col, Divider, Form, Grid, Input, InputNumber, Modal,
    Row, Select, Table, Tag, Tabs, Typography, message, Space, Popconfirm, Spin,
} from "antd";
import { DeleteOutlined, SearchOutlined } from "@ant-design/icons";
import type { ColumnsType } from "antd/es/table";
import { useUiCommandStore } from "../../stores/ui-command.store";
import { getDepartmentOptions, type DepartmentOptionDto } from "../../api/department.api";
import {
    createLiquidation, updateLiquidation, searchLiquidations, getLiquidation,
    confirmLiquidation, cancelLiquidation, addLiquidationAssets, removeLiquidationAsset,
    searchScrappedAssets,
    LiquidationStatus, LIQUIDATION_STATUS_CONFIG,
    type LiquidationOrderDto, type LiquidationOrderListDto,
    type LiquidationDetailDto, type ScrappedAssetCandidateDto,
    type CreateLiquidationDto, type UpdateLiquidationDto,
} from "../../api/liquidation.api";

const { Text, Title } = Typography;
const { useBreakpoint } = Grid;
const PAGE_SIZE = 20;
const MODAL_ASSET_PAGE_SIZE = 10;

const AssetLiquidationPage: React.FC = () => {
    const screens = useBreakpoint();
    const isMobile = !screens.md;

    const command = useUiCommandStore((s) => s.command);
    const clearCommand = useUiCommandStore((s) => s.clear);

    const [departments, setDepartments] = useState<DepartmentOptionDto[]>([]);
    const [activeTab, setActiveTab] = useState("1");

    const [orders, setOrders] = useState<LiquidationOrderListDto[]>([]);
    const [total, setTotal] = useState(0);
    const [pageIndex, setPageIndex] = useState(1);
    const [loadingList, setLoadingList] = useState(false);

    const [filterDept, setFilterDept] = useState<string | undefined>();
    const [filterStatus, setFilterStatus] = useState<LiquidationStatus | undefined>();

    const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
    const [selectedOrder, setSelectedOrder] = useState<LiquidationOrderDto | null>(null);
    const [loadingDetail, setLoadingDetail] = useState(false);

    const selectedOrderRef = useRef<LiquidationOrderDto | null>(null);
    const openEditModalRef = useRef<(order: LiquidationOrderDto) => void>(() => { });
    const openCreateModalRef = useRef<() => void>(() => { });
    const loadOrdersRef = useRef<(page?: number) => void>(() => { });
    const confirmOpenRef = useRef<(v: boolean) => void>(() => { });
    const cancelOpenRef = useRef<(v: boolean) => void>(() => { });

    /* ── modal tạo ── */
    const [createOpen, setCreateOpen] = useState(false);
    const [saving, setSaving] = useState(false);
    const [formCreate] = Form.useForm();
    const [createDept, setCreateDept] = useState<string | undefined>();
    const [createAssetKeyword, setCreateAssetKeyword] = useState("");
    const [createAssets, setCreateAssets] = useState<ScrappedAssetCandidateDto[]>([]);
    const [createAssetsTotal, setCreateAssetsTotal] = useState(0);
    const [createAssetsPage, setCreateAssetsPage] = useState(1);
    const [loadingCreateAssets, setLoadingCreateAssets] = useState(false);
    const [selectedCreateNos, setSelectedCreateNos] = useState<string[]>([]);

    /* ── modal sửa ── */
    const [editOpen, setEditOpen] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [savingEdit, setSavingEdit] = useState(false);
    const [formEdit] = Form.useForm<UpdateLiquidationDto>();

    /* ── modal confirm / cancel ── */
    const [confirmOpen, setConfirmOpen] = useState(false);
    const [cancelOpen, setCancelOpen] = useState(false);

    /* ── modal thêm tài sản ── */
    const [addAssetOpen, setAddAssetOpen] = useState(false);
    const [candidateAssetNo, setCandidateAssetNo] = useState("");
    const [candidates, setCandidates] = useState<ScrappedAssetCandidateDto[]>([]);
    const [candidateTotal, setCandidateTotal] = useState(0);
    const [candidatePage, setCandidatePage] = useState(1);
    const [loadingCandidates, setLoadingCandidates] = useState(false);
    const [selectedAssetNos, setSelectedAssetNos] = useState<string[]>([]);
    const [addingAssets, setAddingAssets] = useState(false);

    /* ════════════════════════════════════════════════════════════
       PERMISSION HELPERS
    ════════════════════════════════════════════════════════════ */
    const normalizeStatus = (status: any): LiquidationStatus | undefined => {
        if (typeof status === "number") return status as LiquidationStatus;
        if (typeof status === "string") return LiquidationStatus[status as keyof typeof LiquidationStatus] as unknown as LiquidationStatus;
        return undefined;
    };

    const checkCanEdit = (order: LiquidationOrderDto | null) => {
        const s = normalizeStatus(order?.status);
        return s === LiquidationStatus.Draft || s === LiquidationStatus.Pending;
    };
    const checkCanConfirm = (order: LiquidationOrderDto | null) => normalizeStatus(order?.status) === LiquidationStatus.Pending;
    const checkCanCancel = (order: LiquidationOrderDto | null) => {
        const s = normalizeStatus(order?.status);
        return s === LiquidationStatus.Draft || s === LiquidationStatus.Pending;
    };

    const canEdit = checkCanEdit(selectedOrder);

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
            const res = await searchLiquidations({ departmentId: filterDept, status: filterStatus, pageIndex: page, pageSize: PAGE_SIZE });
            setOrders(res.items); setTotal(res.total); setPageIndex(page);
        } catch { message.error("Không tải được danh sách phiếu"); }
        finally { setLoadingList(false); }
    }, [filterDept, filterStatus]);

    useEffect(() => { loadOrders(1); }, [loadOrders]);

    /* ════════════════════════════════════════════════════════════
       LOAD / REFRESH DETAIL
    ════════════════════════════════════════════════════════════ */
    const loadDetail = async (id: string) => {
        setLoadingDetail(true);
        try {
            const res = await getLiquidation(id);
            setSelectedOrder(res); selectedOrderRef.current = res; setSelectedOrderId(id);
        } catch { message.error("Không tải được chi tiết phiếu"); }
        finally { setLoadingDetail(false); }
    };

    const refreshDetail = async () => {
        if (!selectedOrderId) return;
        try {
            const res = await getLiquidation(selectedOrderId);
            setSelectedOrder(res); selectedOrderRef.current = res;
        } catch { /* silent */ }
    };

    /* ════════════════════════════════════════════════════════════
       MODAL TẠO
    ════════════════════════════════════════════════════════════ */
    const openCreateModal = () => {
        formCreate.resetFields();
        setCreateDept(undefined);
        setCreateAssetKeyword("");
        setCreateAssets([]); setCreateAssetsTotal(0); setCreateAssetsPage(1);
        setSelectedCreateNos([]);
        setCreateOpen(true);
    };

    const loadCreateAssets = useCallback(async (deptId: string | undefined, keyword: string, page: number) => {
        setLoadingCreateAssets(true);
        try {
            const res = await searchScrappedAssets({ departmentId: deptId, assetNo: keyword || undefined, pageIndex: page, pageSize: MODAL_ASSET_PAGE_SIZE });
            setCreateAssets(res.items); setCreateAssetsTotal(res.total); setCreateAssetsPage(page);
        } catch { message.error("Không tải được danh sách tài sản"); }
        finally { setLoadingCreateAssets(false); }
    }, []);

    const handleCreateDeptChange = async (v: string | undefined) => {
        setCreateDept(v);
        setSelectedCreateNos([]);
        setCreateAssetsPage(1);
        loadCreateAssets(v, createAssetKeyword, 1);
    };

    const handleCreateSubmit = async () => {
        if (!selectedCreateNos.length) { message.warning("Chọn ít nhất 1 tài sản để thanh lý"); return; }
        try {
            const values = await formCreate.validateFields();
            setSaving(true);
            const created = await createLiquidation({
                departmentId: values.departmentId,
                liquidationMethod: values.liquidationMethod,
                recoveredAmount: values.recoveredAmount,
                note: values.note,
                assetNos: selectedCreateNos,
            } as CreateLiquidationDto);
            message.success("Tạo phiếu thanh lý thành công");
            setCreateOpen(false);
            await loadOrders(1);
            await loadDetail(created.id);
        } catch (e: any) {
            if (e?.response) message.error(e.response.data?.message ?? "Lưu thất bại");
        } finally { setSaving(false); }
    };

    /* ════════════════════════════════════════════════════════════
       MODAL SỬA
    ════════════════════════════════════════════════════════════ */
    const openEditModal = (order: LiquidationOrderDto) => {
        setEditingId(order.id);
        formEdit.setFieldsValue({
            liquidationMethod: order.liquidationMethod,
            recoveredAmount: order.recoveredAmount,
            note: order.note,
        });
        setEditOpen(true);
    };

    const handleEditSubmit = async (values: any) => {
        if (!editingId) return;
        setSavingEdit(true);
        try {
            await updateLiquidation(editingId, values as UpdateLiquidationDto);
            message.success("Cập nhật phiếu thành công");
            setEditOpen(false);
            await refreshDetail();
        } catch (e: any) { message.error(e?.response?.data?.message ?? "Lưu thất bại"); }
        finally { setSavingEdit(false); }
    };

    /* ════════════════════════════════════════════════════════════
       CONFIRM / CANCEL
    ════════════════════════════════════════════════════════════ */
    const handleConfirm = async () => {
        if (!selectedOrder) return;
        try {
            await confirmLiquidation(selectedOrder.id);
            message.success("Xác nhận thanh lý thành công");
            setConfirmOpen(false);
            await Promise.all([loadOrders(pageIndex), refreshDetail()]);
        } catch (e: any) { message.error(e?.response?.data?.message ?? "Xác nhận thất bại"); }
    };

    const handleCancel = async () => {
        if (!selectedOrder) return;
        try {
            await cancelLiquidation(selectedOrder.id);
            message.success("Đã hủy phiếu thanh lý");
            setCancelOpen(false);
            await Promise.all([loadOrders(pageIndex), refreshDetail()]);
        } catch (e: any) { message.error(e?.response?.data?.message ?? "Hủy thất bại"); }
    };

    /* ════════════════════════════════════════════════════════════
       ADD ASSETS
    ════════════════════════════════════════════════════════════ */
    const openAddAssetModal = () => {
        setCandidateAssetNo("");
        setCandidates([]); setCandidateTotal(0); setCandidatePage(1);
        setSelectedAssetNos([]);
        setAddAssetOpen(true);
    };

    const loadCandidates = async (page = 1) => {
        setLoadingCandidates(true);
        try {
            const res = await searchScrappedAssets({
                departmentId: selectedOrder?.departmentId,
                assetNo: candidateAssetNo || undefined,
                pageIndex: page, pageSize: PAGE_SIZE,
            });
            setCandidates(res.items); setCandidateTotal(res.total); setCandidatePage(page);
        } catch { message.error("Tìm tài sản thất bại"); }
        finally { setLoadingCandidates(false); }
    };

    const handleAddAssets = async () => {
        if (!selectedOrder || !selectedAssetNos.length) return;
        setAddingAssets(true);
        try {
            await addLiquidationAssets(selectedOrder.id, { assetNos: selectedAssetNos });
            message.success(`Đã thêm ${selectedAssetNos.length} tài sản vào phiếu`);
            setAddAssetOpen(false);
            await refreshDetail();
        } catch (e: any) { message.error(e?.response?.data?.message ?? "Thêm tài sản thất bại"); }
        finally { setAddingAssets(false); }
    };

    const handleRemoveDetail = async (detailId: number) => {
        if (!selectedOrder) return;
        try {
            await removeLiquidationAsset(selectedOrder.id, detailId);
            message.success("Đã xóa tài sản khỏi phiếu");
            await refreshDetail();
        } catch (e: any) { message.error(e?.response?.data?.message ?? "Xóa thất bại"); }
    };

    /* ════════════════════════════════════════════════════════════
       REFS & TOOLBAR
    ════════════════════════════════════════════════════════════ */
    useEffect(() => {
        selectedOrderRef.current = selectedOrder;
        openEditModalRef.current = openEditModal;
        openCreateModalRef.current = openCreateModal;
        loadOrdersRef.current = loadOrders;
        confirmOpenRef.current = setConfirmOpen;
        cancelOpenRef.current = setCancelOpen;
    });

    useEffect(() => {
        if (!command) return;
        const order = selectedOrderRef.current;
        switch (command) {
            case "QUERY": loadOrdersRef.current(1); break;
            case "INSERT":
                if (activeTab === "1") openCreateModalRef.current();
                else if (activeTab === "2") {
                    if (checkCanEdit(order)) openAddAssetModal();
                    else message.warning("Chọn phiếu Nháp hoặc Chờ duyệt để thêm tài sản");
                }
                break;
            case "MODIFY":
                if (checkCanEdit(order)) openEditModalRef.current(order!);
                else message.warning("Chọn phiếu Nháp hoặc Chờ duyệt để sửa");
                break;
            case "CONFIRM":
                if (checkCanConfirm(order)) confirmOpenRef.current(true);
                else message.warning("Chọn phiếu Chờ duyệt để xác nhận thanh lý");
                break;
            case "DELETE":
                if (checkCanCancel(order)) cancelOpenRef.current(true);
                else message.warning("Chọn phiếu Nháp hoặc Chờ duyệt để hủy");
                break;
            default: break;
        }
        clearCommand();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [command, activeTab]);

    /* ════════════════════════════════════════════════════════════
       COLUMNS
    ════════════════════════════════════════════════════════════ */
    const deptOptions = departments.map((d) => ({ value: d.id, label: d.name }));
    const statusOptions = Object.entries(LIQUIDATION_STATUS_CONFIG).map(([k, v]) => ({ value: Number(k), label: v.label }));
    const filterOption = (input: string, option?: { label: string; value: string }) =>
        (option?.label ?? "").toLowerCase().includes(input.toLowerCase());

    const orderColumns: ColumnsType<LiquidationOrderListDto> = [
        { title: "Mã phiếu", dataIndex: "id", key: "id", width: 160, fixed: "left" as const, render: (id) => <a onClick={() => loadDetail(id)}>{id}</a> },
        { title: "Đơn vị", dataIndex: "departmentName", key: "dept", ellipsis: true },
        { title: "Tài sản", dataIndex: "assetCount", key: "count", width: 80, align: "center" },
        {
            title: "Trạng thái", dataIndex: "status", key: "status", width: 140,
            render: (s: LiquidationStatus) => {
                const cfg = LIQUIDATION_STATUS_CONFIG[s];
                return <Tag color={cfg?.color ?? "default"}>{cfg?.label ?? `(${s})`}</Tag>;
            },
        },
        { title: "Người tạo", dataIndex: "createdByUserName", key: "creator", width: 130 },
        { title: "Ngày tạo", dataIndex: "createdAt", key: "createdAt", width: 110, render: (v: string) => new Date(v).toLocaleDateString("vi-VN") },
    ];

    const detailColumns: ColumnsType<LiquidationDetailDto> = [
        { title: "Mã tài sản", dataIndex: "assetNo", key: "assetNo", width: 140 },
        { title: "Tên tài sản", dataIndex: "assetName", key: "assetName", ellipsis: true },
        { title: "Model", dataIndex: "modelName", key: "model", width: 160, ellipsis: true },
        { title: "Phiếu báo phế", dataIndex: "scrapId", key: "scrapId", width: 140 },
        { title: "ĐV trước đó", dataIndex: "previousUseDepartmentId", key: "prevDept", width: 150, render: (id: string) => getDeptName(id) },
        {
            title: "", key: "action", width: 60, align: "center",
            render: (_, row) => !canEdit ? null : (
                <Popconfirm title="Xóa tài sản này?" onConfirm={() => handleRemoveDetail(row.id)}>
                    <Button type="text" danger icon={<DeleteOutlined />} size="small" />
                </Popconfirm>
            ),
        },
    ];

    const assetPickColumns: ColumnsType<ScrappedAssetCandidateDto> = [
        { title: "Mã tài sản", dataIndex: "assetNo", key: "assetNo", width: 140 },
        { title: "Tên tài sản", dataIndex: "assetName", key: "assetName", ellipsis: true },
        { title: "Model", dataIndex: "modelName", key: "model", width: 160, ellipsis: true },
        { title: "Phiếu báo phế", dataIndex: "scrapId", key: "scrapId", width: 140 },
    ];

    const tabItems = [
        {
            key: "1", label: "Danh sách phiếu",
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
                        pagination={{ total, pageSize: PAGE_SIZE, current: pageIndex, onChange: (p) => loadOrders(p), showSizeChanger: false, showTotal: (t) => `Tổng ${t} phiếu` }} />
                </>
            ),
        },
        {
            key: "2", label: "Chi tiết phiếu",
            children: (
                <>
                    {!selectedOrderId && <div style={{ padding: 24, textAlign: "center" }}><Text type="secondary">Chọn một phiếu từ tab "Danh sách phiếu" để xem chi tiết.</Text></div>}
                    {loadingDetail && selectedOrderId && <div style={{ textAlign: "center", padding: 24 }}><Spin /></div>}
                    {selectedOrder && !loadingDetail && (
                        <>
                            <Card size="small" style={{ marginBottom: 12 }}>
                                <Row gutter={[16, 8]}>
                                    <Col xs={12} md={4}><Text type="secondary">Mã phiếu</Text><br /><Text strong>{selectedOrder.id}</Text></Col>
                                    <Col xs={12} md={4}>
                                        <Text type="secondary">Trạng thái</Text><br />
                                        <Tag color={LIQUIDATION_STATUS_CONFIG[selectedOrder.status]?.color ?? "default"}>
                                            {LIQUIDATION_STATUS_CONFIG[selectedOrder.status]?.label ?? `(${selectedOrder.status})`}
                                        </Tag>
                                    </Col>
                                    <Col xs={12} md={4}><Text type="secondary">Đơn vị</Text><br /><Text>{selectedOrder.departmentName}</Text></Col>
                                    {selectedOrder.liquidationMethod && <Col xs={12} md={4}><Text type="secondary">Hình thức</Text><br /><Text>{selectedOrder.liquidationMethod}</Text></Col>}
                                    {selectedOrder.recoveredAmount != null && <Col xs={12} md={4}><Text type="secondary">Giá trị thu hồi</Text><br /><Text>{selectedOrder.recoveredAmount.toLocaleString("vi-VN")} đ</Text></Col>}
                                    {selectedOrder.confirmedAt && <Col xs={12} md={4}><Text type="secondary">Ngày thanh lý</Text><br /><Text>{new Date(selectedOrder.confirmedAt).toLocaleDateString("vi-VN")}</Text></Col>}
                                    <Col xs={24} md={8}><Text type="secondary">Ghi chú</Text><br /><Text>{selectedOrder.note ?? "—"}</Text></Col>
                                    <Col xs={12} md={4}><Text type="secondary">Người tạo</Text><br /><Text>{selectedOrder.createdByUserName}</Text></Col>
                                    {selectedOrder.confirmedByUserName && <Col xs={12} md={4}><Text type="secondary">Người xác nhận</Text><br /><Text>{selectedOrder.confirmedByUserName}</Text></Col>}
                                </Row>
                            </Card>
                            <Divider style={{ margin: "12px 0" }}>Danh sách tài sản ({selectedOrder.details.length})</Divider>
                            <Table rowKey="id" dataSource={selectedOrder.details} columns={detailColumns} size="small" scroll={{ x: 700 }} pagination={false} />
                        </>
                    )}
                </>
            ),
        },
    ];

    return (
        <div style={{ padding: isMobile ? 8 : 16 }}>
            <Title level={4} style={{ marginBottom: 16 }}>♻️ Thanh lý tài sản</Title>
            <Tabs activeKey={activeTab} onChange={setActiveTab} items={tabItems} size={isMobile ? "small" : "middle"} />

            {/* ══ MODAL TẠO ══ */}
            <Modal title="Tạo phiếu thanh lý" open={createOpen} onCancel={() => setCreateOpen(false)}
                onOk={handleCreateSubmit} okText={`Tạo phiếu${selectedCreateNos.length ? ` (${selectedCreateNos.length} tài sản)` : ""}`}
                okButtonProps={{ disabled: !selectedCreateNos.length }} confirmLoading={saving} width={isMobile ? "98vw" : 900} destroyOnClose>
                <Form form={formCreate} layout="vertical">
                    <Row gutter={12}>
                        <Col xs={24} sm={12}>
                            <Form.Item name="departmentId" label="Đơn vị" rules={[{ required: true, message: "Chọn đơn vị" }]}>
                                <Select options={deptOptions} placeholder="Chọn đơn vị" showSearch filterOption={filterOption}
                                    onChange={(v) => handleCreateDeptChange(v)} />
                            </Form.Item>
                        </Col>
                        <Col xs={24} sm={12}>
                            <Form.Item name="liquidationMethod" label="Hình thức thanh lý">
                                <Input placeholder="VD: Bán, Tiêu hủy, Cho tặng..." />
                            </Form.Item>
                        </Col>
                        <Col xs={24} sm={12}>
                            <Form.Item name="recoveredAmount" label="Giá trị thu hồi (VNĐ)">
                                <InputNumber style={{ width: "100%" }} min={0} formatter={(v) => `${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",")} />
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
                    Chọn tài sản đã báo phế để thanh lý
                    {selectedCreateNos.length > 0 && <Tag color="blue" style={{ marginLeft: 8 }}>Đã chọn: {selectedCreateNos.length}</Tag>}
                </Divider>

                {!createDept ? (
                    <div style={{ textAlign: "center", color: "#999", padding: "16px 0" }}>Chọn đơn vị để xem danh sách tài sản đã báo phế</div>
                ) : (
                    <>
                        <Row gutter={[8, 8]} style={{ marginBottom: 8 }}>
                            <Col xs={24} sm={16}>
                                <Input placeholder="Tìm theo mã tài sản" value={createAssetKeyword}
                                    onChange={(e) => setCreateAssetKeyword(e.target.value)}
                                    onPressEnter={() => { setCreateAssetsPage(1); loadCreateAssets(createDept, createAssetKeyword, 1); }} allowClear />
                            </Col>
                            <Col xs={24} sm={8}>
                                <Button icon={<SearchOutlined />} block onClick={() => { setCreateAssetsPage(1); loadCreateAssets(createDept, createAssetKeyword, 1); }}>Tìm</Button>
                            </Col>
                        </Row>
                        <Table rowKey="assetNo" dataSource={createAssets} columns={assetPickColumns}
                            loading={loadingCreateAssets} size="small" scroll={{ x: 560 }}
                            rowSelection={{ selectedRowKeys: selectedCreateNos, onChange: (keys) => setSelectedCreateNos(keys as string[]) }}
                            pagination={{ total: createAssetsTotal, pageSize: MODAL_ASSET_PAGE_SIZE, current: createAssetsPage, onChange: (p) => loadCreateAssets(createDept, createAssetKeyword, p), showSizeChanger: false, showTotal: (t) => `${t} tài sản` }} />
                    </>
                )}
            </Modal>

            {/* ══ MODAL SỬA ══ */}
            <Modal title="Sửa phiếu thanh lý" open={editOpen} onCancel={() => setEditOpen(false)}
                onOk={() => formEdit.submit()} okText="Lưu" confirmLoading={savingEdit} width={isMobile ? "95vw" : 560} destroyOnClose>
                <Form form={formEdit} layout="vertical" onFinish={handleEditSubmit}>
                    <Form.Item name="liquidationMethod" label="Hình thức thanh lý"><Input /></Form.Item>
                    <Form.Item name="recoveredAmount" label="Giá trị thu hồi (VNĐ)"><InputNumber style={{ width: "100%" }} min={0} /></Form.Item>
                    <Form.Item name="note" label="Ghi chú"><Input.TextArea rows={2} /></Form.Item>
                </Form>
            </Modal>

            {/* ══ MODAL XÁC NHẬN ══ */}
            <Modal title="Xác nhận thanh lý?" open={confirmOpen} onOk={handleConfirm} onCancel={() => setConfirmOpen(false)} okText="Xác nhận thanh lý" okButtonProps={{ danger: true }}>
                <p>Xác nhận thanh lý <strong>{selectedOrder?.details?.length ?? 0}</strong> tài sản thuộc <strong>{selectedOrder?.departmentName}</strong>.</p>
                <p style={{ color: "#ff4d4f", fontWeight: "bold" }}>Hành động này không thể hoàn tác.</p>
            </Modal>

            {/* ══ MODAL HỦY ══ */}
            <Modal title="Hủy phiếu thanh lý?" open={cancelOpen} onOk={handleCancel} onCancel={() => setCancelOpen(false)} okText="Hủy phiếu" okButtonProps={{ danger: true }}>
                <p>Phiếu <strong>{selectedOrder?.id}</strong> sẽ bị hủy và không thể khôi phục.</p>
            </Modal>

            {/* ══ MODAL THÊM TÀI SẢN ══ */}
            <Modal title="Thêm tài sản vào phiếu" open={addAssetOpen} onCancel={() => setAddAssetOpen(false)} width={820}
                footer={
                    <Space>
                        <Button onClick={() => setAddAssetOpen(false)}>Đóng</Button>
                        <Button type="primary" loading={addingAssets} disabled={!selectedAssetNos.length} onClick={handleAddAssets}>
                            Thêm {selectedAssetNos.length > 0 ? `(${selectedAssetNos.length})` : ""}
                        </Button>
                    </Space>
                } destroyOnClose>
                <Row gutter={[8, 8]} style={{ marginBottom: 12 }}>
                    <Col xs={24} sm={12}>
                        <Input placeholder="Mã tài sản" value={candidateAssetNo}
                            onChange={(e) => setCandidateAssetNo(e.target.value)}
                            onPressEnter={() => loadCandidates(1)} allowClear />
                    </Col>
                    <Col xs={24} sm={12}>
                        <Button icon={<SearchOutlined />} type="primary" onClick={() => loadCandidates(1)} block>Tìm kiếm</Button>
                    </Col>
                </Row>
                <Table rowKey="assetNo" dataSource={candidates} columns={assetPickColumns}
                    loading={loadingCandidates} size="small" scroll={{ x: 620 }}
                    rowSelection={{
                        selectedRowKeys: selectedAssetNos,
                        onChange: (keys) => setSelectedAssetNos(keys as string[]),
                        getCheckboxProps: (row) => ({ disabled: selectedOrder?.details.some((d) => d.assetNo === row.assetNo) }),
                    }}
                    pagination={{ total: candidateTotal, pageSize: PAGE_SIZE, current: candidatePage, onChange: (p) => loadCandidates(p), showSizeChanger: false, showTotal: (t) => `${t} tài sản` }} />
            </Modal>
        </div>
    );
};

export default AssetLiquidationPage;