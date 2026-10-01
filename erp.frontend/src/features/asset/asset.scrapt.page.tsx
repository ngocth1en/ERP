import React, { useEffect, useState, useCallback, useRef } from "react";
import {
    Button, Card, Col, Divider, Form, Grid, Input, Modal,
    Row, Select, Table, Tag, Tabs, Typography, message, Space, Popconfirm, Spin,
} from "antd";
import { DeleteOutlined, PlusOutlined, SearchOutlined } from "@ant-design/icons";
import type { ColumnsType } from "antd/es/table";
import { useUiCommandStore } from "../../stores/ui-command.store";
import { getDepartmentOptions, type DepartmentOptionDto } from "../../api/department.api";
import {
    getAllStorageLocations, searchStorageLocations,
    type StorageLocationDto,
} from "../../api/location.api";
import {
    createScrap, updateScrap, searchScraps, getScrap,
    confirmScrap, cancelScrap, addScrapAssets, removeScrapAsset,
    searchCandidateAssets,
    ScrapStatus, SCRAP_STATUS_CONFIG,
    type ScrapOrderDto, type ScrapOrderListDto,
    type ScrapDetailDto, type ScrapAssetCandidateDto,
    type CreateScrapDto, type UpdateScrapDto,
} from "../../api/scrapt.api";

const { Text, Title } = Typography;
const { useBreakpoint } = Grid;
const PAGE_SIZE = 20;
const MODAL_ASSET_PAGE_SIZE = 10;

type LocOption = { value: string; label: string };

const AssetScrapPage: React.FC = () => {
    const screens = useBreakpoint();
    const isMobile = !screens.md;

    /* ── toolbar ── */
    const command = useUiCommandStore((s) => s.command);
    const clearCommand = useUiCommandStore((s) => s.clear);

    /* ── lookup ── */
    const [departments, setDepartments] = useState<DepartmentOptionDto[]>([]);
    const [allStorageLocs, setAllStorageLocs] = useState<StorageLocationDto[]>([]);

    /* ── tabs ── */
    const [activeTab, setActiveTab] = useState("1");

    /* ── danh sách phiếu ── */
    const [orders, setOrders] = useState<ScrapOrderListDto[]>([]);
    const [total, setTotal] = useState(0);
    const [pageIndex, setPageIndex] = useState(1);
    const [loadingList, setLoadingList] = useState(false);

    /* ── filter ── */
    const [filterDept, setFilterDept] = useState<string | undefined>();
    const [filterStatus, setFilterStatus] = useState<ScrapStatus | undefined>();

    /* ── chi tiết ── */
    const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
    const [selectedOrder, setSelectedOrder] = useState<ScrapOrderDto | null>(null);
    const [loadingDetail, setLoadingDetail] = useState(false);

    // ✅ Ref luôn giữ giá trị mới nhất để dùng trong toolbar effect
    const selectedOrderRef = useRef<ScrapOrderDto | null>(null);
    const openEditModalRef = useRef<(order: ScrapOrderDto) => void>(() => { });
    const openCreateModalRef = useRef<() => void>(() => { });
    const loadOrdersRef = useRef<(page?: number) => void>(() => { });
    const confirmOpenRef = useRef<(v: boolean) => void>(() => { });
    const cancelOpenRef = useRef<(v: boolean) => void>(() => { });

    /* ── modal tạo ── */
    const [createOpen, setCreateOpen] = useState(false);
    const [saving, setSaving] = useState(false);
    const [formCreate] = Form.useForm();

    const [createDept, setCreateDept] = useState<string | undefined>();
    const [createLoc, setCreateLoc] = useState<string | undefined>();
    const [createAssetKeyword, setCreateAssetKeyword] = useState<string>("");
    const [createAssets, setCreateAssets] = useState<ScrapAssetCandidateDto[]>([]);
    const [createAssetsTotal, setCreateAssetsTotal] = useState(0);
    const [createAssetsPage, setCreateAssetsPage] = useState(1);
    const [loadingCreateAssets, setLoadingCreateAssets] = useState(false);
    const [selectedCreateNos, setSelectedCreateNos] = useState<string[]>([]);
    const [createLocOptions, setCreateLocOptions] = useState<LocOption[]>([]);

    /* ── modal sửa ── */
    const [editOpen, setEditOpen] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [savingEdit, setSavingEdit] = useState(false);
    const [formEdit] = Form.useForm<UpdateScrapDto>();

    /* ── modal confirm / cancel ── */
    const [confirmOpen, setConfirmOpen] = useState(false);
    const [cancelOpen, setCancelOpen] = useState(false);

    /* ── modal thêm tài sản ── */
    const [addAssetOpen, setAddAssetOpen] = useState(false);
    const [candidateDept, setCandidateDept] = useState<string>("");
    const [candidateLocation, setCandidateLocation] = useState<string | undefined>();
    const [candidateAssetNo, setCandidateAssetNo] = useState<string>("");
    const [candidates, setCandidates] = useState<ScrapAssetCandidateDto[]>([]);
    const [candidateTotal, setCandidateTotal] = useState(0);
    const [candidatePage, setCandidatePage] = useState(1);
    const [loadingCandidates, setLoadingCandidates] = useState(false);
    const [selectedAssetNos, setSelectedAssetNos] = useState<string[]>([]);
    const [addingAssets, setAddingAssets] = useState(false);

    /* ════════════════════════════════════════════════════════════
       PERMISSION FLAGS & HELPERS
    ════════════════════════════════════════════════════════════ */
    const normalizeStatus = (status: any): ScrapStatus | undefined => {
        if (typeof status === "number") return status as ScrapStatus;
        if (typeof status === "string") {
            return ScrapStatus[status as keyof typeof ScrapStatus] as unknown as ScrapStatus;
        }
        return undefined;
    };

    const checkCanEdit = (order: ScrapOrderDto | null): boolean => {
        if (!order) return false;
        const status = normalizeStatus(order.status);
        return status === ScrapStatus.Draft || status === ScrapStatus.Pending;
    };

    const checkCanConfirm = (order: ScrapOrderDto | null): boolean => {
        if (!order) return false;
        const status = normalizeStatus(order.status);
        return status === ScrapStatus.Pending;
    };

    const checkCanCancel = (order: ScrapOrderDto | null): boolean => {
        if (!order) return false;
        const status = normalizeStatus(order.status);
        return status === ScrapStatus.Draft || status === ScrapStatus.Pending;
    };

    const canEdit = checkCanEdit(selectedOrder);
    const canConfirm = checkCanConfirm(selectedOrder);
    const canCancel = checkCanCancel(selectedOrder);

    /* ════════════════════════════════════════════════════════════
       LOAD LOOKUP
    ════════════════════════════════════════════════════════════ */
    useEffect(() => {
        Promise.all([
            getDepartmentOptions().then((r) => setDepartments(r.data)),
            getAllStorageLocations().then((r) => {
                setAllStorageLocs(r.data);
            }),
        ]).catch(() => message.error("Không tải được dữ liệu lookup"));
    }, []);

    /* helpers */
    const getDeptName = (id?: string) =>
        departments.find((d) => d.id === id)?.name ?? id ?? "—";
    const getLocName = (no?: string | null) =>
        allStorageLocs.find((l) => l.storageLocationNo === no)?.nameVN ?? no ?? "—";

    const loadLocOptionsByDept = async (deptId: string | undefined): Promise<LocOption[]> => {
        if (!deptId) return [];
        try {
            const res = await searchStorageLocations({ departmentId: deptId, page: 1, pageSize: 200 });
            return res.data.items.map((l) => ({ value: l.storageLocationNo, label: l.nameVN }));
        } catch { return []; }
    };

    /* ════════════════════════════════════════════════════════════
       LOAD ORDERS
    ════════════════════════════════════════════════════════════ */
    const loadOrders = useCallback(async (page = 1) => {
        setLoadingList(true);
        try {
            const res = await searchScraps({
                departmentId: filterDept,
                status: filterStatus,
                pageIndex: page,
                pageSize: PAGE_SIZE,
            });
            setOrders(res.items);
            setTotal(res.total);
            setPageIndex(page);
        } catch {
            message.error("Không tải được danh sách phiếu");
        } finally {
            setLoadingList(false);
        }
    }, [filterDept, filterStatus]);

    useEffect(() => { loadOrders(1); }, [loadOrders]);

    /* ════════════════════════════════════════════════════════════
       LOAD / REFRESH DETAIL
    ════════════════════════════════════════════════════════════ */
    const loadDetail = async (id: string) => {
        setLoadingDetail(true);
        try {
            const res = await getScrap(id);
            setSelectedOrder(res);
            selectedOrderRef.current = res;
            setSelectedOrderId(id);
        } catch {
            message.error("Không tải được chi tiết phiếu");
        } finally {
            setLoadingDetail(false);
        }
    };

    const refreshDetail = async () => {
        if (!selectedOrderId) return;
        try {
            const res = await getScrap(selectedOrderId);
            setSelectedOrder(res);
            selectedOrderRef.current = res;
        }
        catch { /* silent */ }
    };

    /* ════════════════════════════════════════════════════════════
       MODAL TẠO
    ════════════════════════════════════════════════════════════ */
    const openCreateModal = () => {
        formCreate.resetFields();
        setCreateDept(undefined);
        setCreateLoc(undefined);
        setCreateAssetKeyword("");
        setCreateAssets([]);
        setCreateAssetsTotal(0);
        setCreateAssetsPage(1);
        setSelectedCreateNos([]);
        setCreateLocOptions([]);
        setCreateOpen(true);
    };

    const loadCreateAssets = useCallback(async (
        deptId: string | undefined,
        locNo: string | undefined,
        keyword: string,
        page: number,
    ) => {
        if (!deptId) { setCreateAssets([]); setCreateAssetsTotal(0); return; }
        setLoadingCreateAssets(true);
        try {
            const res = await searchCandidateAssets({
                departmentId: deptId,
                storageLocationNo: locNo,
                assetNo: keyword || undefined,
                pageIndex: page,
                pageSize: MODAL_ASSET_PAGE_SIZE,
            });
            setCreateAssets(res.items);
            setCreateAssetsTotal(res.total);
            setCreateAssetsPage(page);
        } catch {
            message.error("Không tải được danh sách tài sản");
        } finally {
            setLoadingCreateAssets(false);
        }
    }, []);

    const handleCreateDeptChange = async (v: string | undefined) => {
        setCreateDept(v);
        setCreateLoc(undefined);
        formCreate.setFieldValue("storageLocationNo", undefined);
        setSelectedCreateNos([]);
        setCreateAssetsPage(1);
        setCreateLocOptions(await loadLocOptionsByDept(v));
        loadCreateAssets(v, undefined, createAssetKeyword, 1);
    };

    const handleCreateLocChange = (v: string | undefined) => {
        setCreateLoc(v);
        setSelectedCreateNos([]);
        setCreateAssetsPage(1);
        loadCreateAssets(createDept, v, createAssetKeyword, 1);
    };

    const handleCreateSubmit = async () => {
        if (!selectedCreateNos.length) {
            message.warning("Chọn ít nhất 1 tài sản để báo phế");
            return;
        }
        try {
            const values = await formCreate.validateFields();
            setSaving(true);
            const created = await createScrap({
                departmentId: values.departmentId,
                storageLocationNo: values.storageLocationNo,
                reason: values.reason,
                note: values.note,
                assetNos: selectedCreateNos,
            } as CreateScrapDto);
            message.success("Tạo phiếu báo phế thành công");
            setCreateOpen(false);
            await loadOrders(1);
            await loadDetail(created.id);
        } catch (e: any) {
            if (e?.response) message.error(e.response.data?.message ?? "Lưu thất bại");
        } finally {
            setSaving(false);
        }
    };

    /* ════════════════════════════════════════════════════════════
       MODAL SỬA
    ════════════════════════════════════════════════════════════ */
    const openEditModal = async (order: ScrapOrderDto) => {
        setEditingId(order.id);
        formEdit.setFieldsValue({
            reason: order.reason,
            note: order.note,
        });
        setEditOpen(true);
    };

    const handleEditSubmit = async (values: any) => {
        if (!editingId) return;
        setSavingEdit(true);
        try {
            await updateScrap(editingId, values as UpdateScrapDto);
            message.success("Cập nhật phiếu thành công");
            setEditOpen(false);
            await refreshDetail();
        } catch (e: any) {
            message.error(e?.response?.data?.message ?? "Lưu thất bại");
        } finally {
            setSavingEdit(false);
        }
    };

    /* ════════════════════════════════════════════════════════════
       CONFIRM / CANCEL
    ════════════════════════════════════════════════════════════ */
    const handleConfirm = async () => {
        if (!selectedOrder) return;
        try {
            await confirmScrap(selectedOrder.id);
            message.success("Xác nhận báo phế thành công");
            setConfirmOpen(false);
            await Promise.all([loadOrders(pageIndex), refreshDetail()]);
        } catch (e: any) {
            message.error(e?.response?.data?.message ?? "Xác nhận thất bại");
        }
    };

    const handleCancel = async () => {
        if (!selectedOrder) return;
        try {
            await cancelScrap(selectedOrder.id);
            message.success("Đã hủy phiếu báo phế");
            setCancelOpen(false);
            await Promise.all([loadOrders(pageIndex), refreshDetail()]);
        } catch (e: any) {
            message.error(e?.response?.data?.message ?? "Hủy thất bại");
        }
    };

    /* ════════════════════════════════════════════════════════════
       ADD ASSETS
    ════════════════════════════════════════════════════════════ */
    const openAddAssetModal = () => {
        setCandidateDept(selectedOrder?.departmentId ?? "");
        setCandidateLocation(selectedOrder?.storageLocationNo ?? undefined);
        setCandidateAssetNo("");
        setCandidates([]);
        setCandidateTotal(0);
        setCandidatePage(1);
        setSelectedAssetNos([]);
        setAddAssetOpen(true);
    };

    const loadCandidates = async (page = 1) => {
        if (!candidateDept) return;
        setLoadingCandidates(true);
        try {
            const res = await searchCandidateAssets({
                departmentId: candidateDept,
                storageLocationNo: candidateLocation,
                assetNo: candidateAssetNo || undefined,
                pageIndex: page,
                pageSize: PAGE_SIZE,
            });
            setCandidates(res.items);
            setCandidateTotal(res.total);
            setCandidatePage(page);
        } catch {
            message.error("Tìm tài sản thất bại");
        } finally {
            setLoadingCandidates(false);
        }
    };

    const handleAddAssets = async () => {
        if (!selectedOrder || !selectedAssetNos.length) return;
        setAddingAssets(true);
        try {
            await addScrapAssets(selectedOrder.id, { assetNos: selectedAssetNos });
            message.success(`Đã thêm ${selectedAssetNos.length} tài sản vào phiếu`);
            setAddAssetOpen(false);
            await refreshDetail();
        } catch (e: any) {
            message.error(e?.response?.data?.message ?? "Thêm tài sản thất bại");
        } finally {
            setAddingAssets(false);
        }
    };

    const handleRemoveDetail = async (detailId: number) => {
        if (!selectedOrder) return;
        try {
            await removeScrapAsset(selectedOrder.id, detailId);
            message.success("Đã xóa tài sản khỏi phiếu");
            await refreshDetail();
        } catch (e: any) {
            message.error(e?.response?.data?.message ?? "Xóa thất bại");
        }
    };

    /* ════════════════════════════════════════════════════════════
       ✅ CẬP NHẬT REFS SAU MỖI RENDER — chống stale closure
    ════════════════════════════════════════════════════════════ */
    useEffect(() => {
        selectedOrderRef.current = selectedOrder;
        openEditModalRef.current = openEditModal;
        openCreateModalRef.current = openCreateModal;
        loadOrdersRef.current = loadOrders;
        confirmOpenRef.current = setConfirmOpen;
        cancelOpenRef.current = setCancelOpen;
    });

    /* ════════════════════════════════════════════════════════════
       TOOLBAR COMMAND — đọc qua ref, không bao giờ stale
    ════════════════════════════════════════════════════════════ */
    useEffect(() => {
        if (!command) return;
        const order = selectedOrderRef.current;

        switch (command) {
            case "QUERY":
                loadOrdersRef.current(1);
                break;
            case "INSERT":
                if (activeTab === "1") {
                    openCreateModalRef.current();
                } else if (activeTab === "2") {
                    if (checkCanEdit(order)) {
                        openAddAssetModal();
                    } else {
                        message.warning("Chọn phiếu Nháp hoặc Chờ xác nhận để thêm tài sản");
                    }
                }
                break;
            case "MODIFY":
                if (checkCanEdit(order)) {
                    openEditModalRef.current(order!);
                } else {
                    message.warning("Chọn phiếu Nháp hoặc Chờ xác nhận để sửa");
                }
                break;
            case "CONFIRM":
                if (checkCanConfirm(order)) {
                    confirmOpenRef.current(true);
                } else {
                    message.warning("Chọn phiếu Chờ xác nhận để xác nhận báo phế");
                }
                break;
            case "DELETE":
                if (checkCanCancel(order)) {
                    cancelOpenRef.current(true);
                } else {
                    message.warning("Chọn phiếu Nháp hoặc Chờ xác nhận để hủy");
                }
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
    const statusOptions = Object.entries(SCRAP_STATUS_CONFIG).map(([k, v]) => ({
        value: Number(k), label: v.label,
    }));
    const filterOption = (input: string, option?: { label: string; value: string }) =>
        (option?.label ?? "").toLowerCase().includes(input.toLowerCase());

    const orderColumns: ColumnsType<ScrapOrderListDto> = [
        {
            title: "Mã phiếu", dataIndex: "id", key: "id", width: 160, fixed: "left" as const,
            render: (id) => <a onClick={() => loadDetail(id)}>{id}</a>,
        },
        { title: "Đơn vị", dataIndex: "departmentName", key: "dept", ellipsis: true },
        { title: "Tài sản", dataIndex: "assetCount", key: "count", width: 80, align: "center" },
        {
            title: "Trạng thái", dataIndex: "status", key: "status", width: 140,
            render: (s: ScrapStatus) => {
                const cfg = SCRAP_STATUS_CONFIG[s];
                return <Tag color={cfg?.color ?? "default"}>{cfg?.label ?? `(${s})`}</Tag>;
            },
        },
        { title: "Người tạo", dataIndex: "createdByUserName", key: "creator", width: 130 },
        {
            title: "Ngày tạo", dataIndex: "createdAt", key: "createdAt", width: 110,
            render: (v: string) => new Date(v).toLocaleDateString("vi-VN"),
        },
    ];

    const detailColumns: ColumnsType<ScrapDetailDto> = [
        { title: "Mã tài sản", dataIndex: "assetNo", key: "assetNo", width: 140 },
        { title: "Tên tài sản", dataIndex: "assetName", key: "assetName", ellipsis: true },
        { title: "Model", dataIndex: "modelName", key: "model", width: 160, ellipsis: true },
        {
            title: "ĐV đang dùng", dataIndex: "previousUseDepartmentId", key: "prevDept", width: 150,
            render: (id: string) => getDeptName(id),
        },
        {
            title: "Vị trí", dataIndex: "previousStorageLocationNo", key: "prevLoc", width: 140,
            render: (no: string) => getLocName(no),
        },
        {
            title: "", key: "action", width: 60, align: "center",
            render: (_, row) => {
                if (!canEdit) return null;
                return (
                    <Popconfirm title="Xóa tài sản này?" onConfirm={() => handleRemoveDetail(row.id)}>
                        <Button type="text" danger icon={<DeleteOutlined />} size="small" />
                    </Popconfirm>
                );
            },
        },
    ];

    const assetPickColumns: ColumnsType<ScrapAssetCandidateDto> = [
        { title: "Mã tài sản", dataIndex: "assetNo", key: "assetNo", width: 140 },
        { title: "Tên tài sản", dataIndex: "assetName", key: "assetName", ellipsis: true },
        { title: "Model", dataIndex: "modelName", key: "model", width: 160, ellipsis: true },
        {
            title: "Vị trí", dataIndex: "storageLocationNo", key: "loc", width: 140,
            render: (no: string) => getLocName(no),
        },
    ];

    /* ════════════════════════════════════════════════════════════
       TAB ITEMS
    ════════════════════════════════════════════════════════════ */
    const tabItems = [
        {
            key: "1",
            label: "Danh sách phiếu",
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

                    <Table
                        rowKey="id"
                        dataSource={orders}
                        columns={orderColumns}
                        loading={loadingList}
                        size="small"
                        scroll={{ x: "max-content" }}
                        onRow={(record) => ({
                            onClick: () => loadDetail(record.id),
                            style: {
                                cursor: "pointer",
                                background: record.id === selectedOrderId ? "#e6f4ff" : undefined,
                            },
                        })}
                        pagination={{
                            total, pageSize: PAGE_SIZE, current: pageIndex,
                            onChange: (p) => loadOrders(p),
                            showSizeChanger: false,
                            showTotal: (t) => `Tổng ${t} phiếu`,
                        }}
                    />
                </>
            ),
        },
        {
            key: "2",
            label: "Chi tiết phiếu",
            children: (
                <>
                    {!selectedOrderId && (
                        <div style={{ padding: 24, textAlign: "center" }}>
                            <Text type="secondary">Chọn một phiếu từ tab "Danh sách phiếu" để xem chi tiết.</Text>
                        </div>
                    )}

                    {loadingDetail && selectedOrderId && (
                        <div style={{ textAlign: "center", padding: 24 }}><Spin /></div>
                    )}

                    {selectedOrder && !loadingDetail && (
                        <>
                            <Card size="small" style={{ marginBottom: 12 }}>
                                <Row gutter={[16, 8]}>
                                    <Col xs={12} md={4}>
                                        <Text type="secondary">Mã phiếu</Text>
                                        <br /><Text strong>{selectedOrder.id}</Text>
                                    </Col>
                                    <Col xs={12} md={4}>
                                        <Text type="secondary">Trạng thái</Text>
                                        <br />
                                        <Tag color={SCRAP_STATUS_CONFIG[selectedOrder.status]?.color ?? "default"}>
                                            {SCRAP_STATUS_CONFIG[selectedOrder.status]?.label ?? `(${selectedOrder.status})`}
                                        </Tag>
                                    </Col>
                                    <Col xs={12} md={4}>
                                        <Text type="secondary">Đơn vị</Text>
                                        <br /><Text>{selectedOrder.departmentName}</Text>
                                    </Col>
                                    {selectedOrder.storageLocationNo && (
                                        <Col xs={12} md={4}>
                                            <Text type="secondary">Vị trí</Text>
                                            <br /><Text>{getLocName(selectedOrder.storageLocationNo)}</Text>
                                        </Col>
                                    )}
                                    {selectedOrder.confirmedAt && (
                                        <Col xs={12} md={4}>
                                            <Text type="secondary">Ngày báo phế</Text>
                                            <br /><Text>{new Date(selectedOrder.confirmedAt).toLocaleDateString("vi-VN")}</Text>
                                        </Col>
                                    )}
                                    <Col xs={24} md={8}>
                                        <Text type="secondary">Lý do</Text>
                                        <br /><Text>{selectedOrder.reason ?? "—"}</Text>
                                    </Col>
                                    <Col xs={24} md={8}>
                                        <Text type="secondary">Ghi chú</Text>
                                        <br /><Text>{selectedOrder.note ?? "—"}</Text>
                                    </Col>
                                    <Col xs={12} md={4}>
                                        <Text type="secondary">Người tạo</Text>
                                        <br /><Text>{selectedOrder.createdByUserName}</Text>
                                    </Col>
                                    {selectedOrder.confirmedByUserName && (
                                        <Col xs={12} md={4}>
                                            <Text type="secondary">Người xác nhận</Text>
                                            <br /><Text>{selectedOrder.confirmedByUserName}</Text>
                                        </Col>
                                    )}
                                </Row>
                            </Card>

                            <Divider style={{ margin: "12px 0" }}>
                                Danh sách tài sản ({selectedOrder.details.length})
                            </Divider>

                            <Table
                                rowKey="id"
                                dataSource={selectedOrder.details}
                                columns={detailColumns}
                                size="small"
                                scroll={{ x: 700 }}
                                pagination={false}
                            />
                        </>
                    )}
                </>
            ),
        },
    ];

    /* ════════════════════════════════════════════════════════════
       RENDER
    ════════════════════════════════════════════════════════════ */
    return (
        <div style={{ padding: isMobile ? 8 : 16 }}>
            <Title level={4} style={{ marginBottom: 16 }}>🗑️ Báo phế tài sản</Title>

            <Tabs activeKey={activeTab} onChange={setActiveTab}
                items={tabItems} size={isMobile ? "small" : "middle"} />

            {/* ══ MODAL TẠO PHIẾU ══ */}
            <Modal title="Tạo phiếu báo phế" open={createOpen}
                onCancel={() => setCreateOpen(false)} onOk={handleCreateSubmit}
                okText={`Tạo phiếu${selectedCreateNos.length ? ` (${selectedCreateNos.length} tài sản)` : ""}`}
                okButtonProps={{ disabled: !selectedCreateNos.length }}
                confirmLoading={saving} width={isMobile ? "98vw" : 900} destroyOnClose>
                <Form form={formCreate} layout="vertical">
                    <Row gutter={12}>
                        <Col xs={24} sm={12}>
                            <Form.Item name="departmentId" label="Đơn vị đang sử dụng"
                                rules={[{ required: true, message: "Chọn đơn vị" }]}>
                                <Select options={deptOptions} placeholder="Chọn đơn vị" showSearch
                                    filterOption={filterOption} onChange={(v) => handleCreateDeptChange(v)} />
                            </Form.Item>
                        </Col>
                        <Col xs={24} sm={12}>
                            <Form.Item name="storageLocationNo" label="Vị trí (tuỳ chọn)">
                                <Select allowClear disabled={!createDept} options={createLocOptions}
                                    placeholder={createDept ? "Chọn vị trí" : "Chọn đơn vị trước"}
                                    showSearch filterOption={filterOption}
                                    onChange={(v) => handleCreateLocChange(v)} />
                            </Form.Item>
                        </Col>
                        <Col xs={24}>
                            <Form.Item name="reason" label="Lý do báo phế">
                                <Input.TextArea rows={2} placeholder="VD: Hỏng, hết khấu hao, mất..." />
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
                    Chọn tài sản cần báo phế
                    {selectedCreateNos.length > 0 && (
                        <Tag color="blue" style={{ marginLeft: 8 }}>Đã chọn: {selectedCreateNos.length}</Tag>
                    )}
                </Divider>

                {!createDept ? (
                    <div style={{ textAlign: "center", color: "#999", padding: "16px 0" }}>
                        Chọn đơn vị để xem danh sách tài sản có thể báo phế
                    </div>
                ) : (
                    <>
                        <Row gutter={[8, 8]} style={{ marginBottom: 8 }}>
                            <Col xs={24} sm={16}>
                                <Input placeholder="Tìm theo mã tài sản" value={createAssetKeyword}
                                    onChange={(e) => setCreateAssetKeyword(e.target.value)}
                                    onPressEnter={() => { setCreateAssetsPage(1); loadCreateAssets(createDept, createLoc, createAssetKeyword, 1); }}
                                    allowClear />
                            </Col>
                            <Col xs={24} sm={8}>
                                <Button icon={<SearchOutlined />} block
                                    onClick={() => { setCreateAssetsPage(1); loadCreateAssets(createDept, createLoc, createAssetKeyword, 1); }}>
                                    Tìm
                                </Button>
                            </Col>
                        </Row>
                        <Table rowKey="assetNo" dataSource={createAssets} columns={assetPickColumns}
                            loading={loadingCreateAssets} size="small" scroll={{ x: 560 }}
                            rowSelection={{
                                selectedRowKeys: selectedCreateNos,
                                onChange: (keys) => setSelectedCreateNos(keys as string[]),
                            }}
                            pagination={{
                                total: createAssetsTotal, pageSize: MODAL_ASSET_PAGE_SIZE,
                                current: createAssetsPage,
                                onChange: (p) => loadCreateAssets(createDept, createLoc, createAssetKeyword, p),
                                showSizeChanger: false, showTotal: (t) => `${t} tài sản`,
                            }} />
                    </>
                )}
            </Modal>

            {/* ══ MODAL SỬA ══ */}
            <Modal title="Sửa phiếu báo phế" open={editOpen}
                onCancel={() => setEditOpen(false)} onOk={() => formEdit.submit()}
                okText="Lưu" confirmLoading={savingEdit}
                width={isMobile ? "95vw" : 560} destroyOnClose>
                <Form form={formEdit} layout="vertical" onFinish={handleEditSubmit}>
                    <Form.Item name="reason" label="Lý do báo phế">
                        <Input.TextArea rows={2} />
                    </Form.Item>
                    <Form.Item name="note" label="Ghi chú">
                        <Input.TextArea rows={2} />
                    </Form.Item>
                </Form>
            </Modal>

            {/* ══ MODAL XÁC NHẬN ══ */}
            <Modal title="Xác nhận báo phế?" open={confirmOpen}
                onOk={handleConfirm} onCancel={() => setConfirmOpen(false)} okText="Xác nhận báo phế"
                okButtonProps={{ danger: true }}>
                <p>
                    Xác nhận báo phế <strong>{selectedOrder?.details?.length ?? 0}</strong> tài sản thuộc{" "}
                    <strong>{selectedOrder?.departmentName}</strong>.
                </p>
                <p style={{ color: "#ff4d4f", fontWeight: "bold" }}>
                    Các tài sản này sẽ bị loại khỏi hệ thống quản lý. Không thể hoàn tác.
                </p>
            </Modal>

            {/* ══ MODAL HỦY ══ */}
            <Modal title="Hủy phiếu báo phế?" open={cancelOpen}
                onOk={handleCancel} onCancel={() => setCancelOpen(false)}
                okText="Hủy phiếu" okButtonProps={{ danger: true }}>
                <p>Phiếu <strong>{selectedOrder?.id}</strong> sẽ bị hủy và không thể khôi phục.</p>
            </Modal>

            {/* ══ MODAL THÊM TÀI SẢN ══ */}
            <Modal title="Thêm tài sản vào phiếu" open={addAssetOpen}
                onCancel={() => setAddAssetOpen(false)} width={820}
                footer={
                    <Space>
                        <Button onClick={() => setAddAssetOpen(false)}>Đóng</Button>
                        <Button type="primary" loading={addingAssets}
                            disabled={!selectedAssetNos.length} onClick={handleAddAssets}>
                            Thêm {selectedAssetNos.length > 0 ? `(${selectedAssetNos.length})` : ""}
                        </Button>
                    </Space>
                }
                destroyOnClose>
                <Card size="small" style={{ marginBottom: 12 }} type="inner">
                    <Row gutter={[16, 8]}>
                        <Col xs={12} md={6}>
                            <Text type="secondary">Đơn vị</Text>
                            <br /><Text strong>{getDeptName(selectedOrder?.departmentId)}</Text>
                        </Col>
                        {selectedOrder?.storageLocationNo && (
                            <Col xs={12} md={6}>
                                <Text type="secondary">Vị trí</Text>
                                <br /><Text>{getLocName(selectedOrder.storageLocationNo)}</Text>
                            </Col>
                        )}
                    </Row>
                </Card>

                <Divider style={{ margin: "12px 0" }}>Tìm tài sản để thêm vào phiếu</Divider>

                <Row gutter={[8, 8]} style={{ marginBottom: 12 }}>
                    <Col xs={24} sm={12}>
                        <Input placeholder="Mã tài sản" value={candidateAssetNo}
                            onChange={(e) => setCandidateAssetNo(e.target.value)}
                            onPressEnter={() => loadCandidates(1)} allowClear />
                    </Col>
                    <Col xs={24} sm={12}>
                        <Button icon={<SearchOutlined />} type="primary"
                            onClick={() => loadCandidates(1)} block>Tìm kiếm</Button>
                    </Col>
                </Row>

                <Table rowKey="assetNo" dataSource={candidates} columns={assetPickColumns}
                    loading={loadingCandidates} size="small" scroll={{ x: 620 }}
                    rowSelection={{
                        selectedRowKeys: selectedAssetNos,
                        onChange: (keys) => setSelectedAssetNos(keys as string[]),
                        getCheckboxProps: (row) => ({
                            disabled: selectedOrder?.details.some((d) => d.assetNo === row.assetNo),
                        }),
                    }}
                    pagination={{
                        total: candidateTotal, pageSize: PAGE_SIZE, current: candidatePage,
                        onChange: (p) => loadCandidates(p), showSizeChanger: false,
                        showTotal: (t) => `${t} tài sản`,
                    }} />
            </Modal>
        </div>
    );
};

export default AssetScrapPage;