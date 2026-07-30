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
    createHandover, updateHandover, searchHandovers, getHandover,
    confirmHandover, cancelHandover, addHandoverAssets, removeHandoverAsset,
    searchCandidateAssets,
    HandoverStatus, HANDOVER_STATUS_CONFIG, calcExpiredAt, isExpired,
    type HandoverOrderDto, type HandoverOrderListDto,
    type HandoverDetailDto, type HandoverAssetCandidateDto,
    type CreateHandoverDto, type UpdateHandoverDto,
} from "../../api/handover.api";

const { Text, Title } = Typography;
const { useBreakpoint } = Grid;
const PAGE_SIZE = 20;
const MODAL_ASSET_PAGE_SIZE = 10;

type LocOption = { value: string; label: string };

const AssetHandoverPage: React.FC = () => {
    const screens = useBreakpoint();
    const isMobile = !screens.md;

    /* ── toolbar ── */
    const command = useUiCommandStore((s) => s.command);
    const clearCommand = useUiCommandStore((s) => s.clear);

    /* ── lookup ── */
    const [departments, setDepartments] = useState<DepartmentOptionDto[]>([]);
    const [allStorageLocs, setAllStorageLocs] = useState<StorageLocationDto[]>([]);
    const [allLocOptions, setAllLocOptions] = useState<LocOption[]>([]);

    /* ── tabs ── */
    const [activeTab, setActiveTab] = useState("1");

    /* ── danh sách phiếu ── */
    const [orders, setOrders] = useState<HandoverOrderListDto[]>([]);
    const [total, setTotal] = useState(0);
    const [pageIndex, setPageIndex] = useState(1);
    const [loadingList, setLoadingList] = useState(false);

    /* ── filter ── */
    const [filterFrom, setFilterFrom] = useState<string | undefined>();
    const [filterTo, setFilterTo] = useState<string | undefined>();
    const [filterStatus, setFilterStatus] = useState<HandoverStatus | undefined>();

    /* ── chi tiết ── */
    const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
    const [selectedOrder, setSelectedOrder] = useState<HandoverOrderDto | null>(null);
    const [loadingDetail, setLoadingDetail] = useState(false);

    // ✅ Ref luôn giữ giá trị mới nhất để dùng trong toolbar effect
    const selectedOrderRef = useRef<HandoverOrderDto | null>(null);
    const openEditModalRef = useRef<(order: HandoverOrderDto) => void>(() => { });
    const openCreateModalRef = useRef<() => void>(() => { });
    const loadOrdersRef = useRef<(page?: number) => void>(() => { });
    const confirmOpenRef = useRef<(v: boolean) => void>(() => { });
    const cancelOpenRef = useRef<(v: boolean) => void>(() => { });

    /* ── modal tạo ── */
    const [createOpen, setCreateOpen] = useState(false);
    const [saving, setSaving] = useState(false);
    const [formCreate] = Form.useForm();

    const [createToDept, setCreateToDept] = useState<string | undefined>();
    const [createFromDept, setCreateFromDept] = useState<string | undefined>();
    const [createFromLoc, setCreateFromLoc] = useState<string | undefined>();
    const [createAssetKeyword, setCreateAssetKeyword] = useState<string>("");
    const [createAssets, setCreateAssets] = useState<HandoverAssetCandidateDto[]>([]);
    const [createAssetsTotal, setCreateAssetsTotal] = useState(0);
    const [createAssetsPage, setCreateAssetsPage] = useState(1);
    const [loadingCreateAssets, setLoadingCreateAssets] = useState(false);
    const [selectedCreateNos, setSelectedCreateNos] = useState<string[]>([]);
    const [fromLocOptions, setFromLocOptions] = useState<LocOption[]>([]);
    const [toLocOptions, setToLocOptions] = useState<LocOption[]>([]);

    /* ── modal sửa ── */
    const [editOpen, setEditOpen] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [savingEdit, setSavingEdit] = useState(false);
    const [formEdit] = Form.useForm<UpdateHandoverDto>();

    /* ── modal confirm / cancel ── */
    const [confirmOpen, setConfirmOpen] = useState(false);
    const [cancelOpen, setCancelOpen] = useState(false);

    /* ── modal thêm tài sản ── */
    const [addAssetOpen, setAddAssetOpen] = useState(false);
    const [candidateFrom, setCandidateFrom] = useState<string>("");
    const [candidateLocation, setCandidateLocation] = useState<string | undefined>();
    const [candidateAssetNo, setCandidateAssetNo] = useState<string>("");
    const [candidates, setCandidates] = useState<HandoverAssetCandidateDto[]>([]);
    const [candidateTotal, setCandidateTotal] = useState(0);
    const [candidatePage, setCandidatePage] = useState(1);
    const [loadingCandidates, setLoadingCandidates] = useState(false);
    const [selectedAssetNos, setSelectedAssetNos] = useState<string[]>([]);
    const [addingAssets, setAddingAssets] = useState(false);

    /* ════════════════════════════════════════════════════════════
       PERMISSION FLAGS
    ════════════════════════════════════════════════════════════ */
    const canEdit = selectedOrder?.status === HandoverStatus.Draft
        || selectedOrder?.status === HandoverStatus.Pending;
    const canConfirm = selectedOrder?.status === HandoverStatus.Pending;
    const canCancel = selectedOrder?.status === HandoverStatus.Draft
        || selectedOrder?.status === HandoverStatus.Pending;

    /* ════════════════════════════════════════════════════════════
       LOAD LOOKUP
    ════════════════════════════════════════════════════════════ */
    useEffect(() => {
        Promise.all([
            getDepartmentOptions().then((r) => setDepartments(r.data)),
            getAllStorageLocations().then((r) => {
                setAllStorageLocs(r.data);
                setAllLocOptions(r.data.map((l: StorageLocationDto) => ({
                    value: l.storageLocationNo,
                    label: l.nameVN ?? l.nameEN,
                })));
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
            const res = await searchHandovers({
                fromDepartmentId: filterFrom,
                toDepartmentId: filterTo,
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
    }, [filterFrom, filterTo, filterStatus]);

    useEffect(() => { loadOrders(1); }, [loadOrders]);

    /* ════════════════════════════════════════════════════════════
       LOAD / REFRESH DETAIL
    ════════════════════════════════════════════════════════════ */
    const loadDetail = async (id: string) => {
        setLoadingDetail(true);
        try {
            const res = await getHandover(id);
            setSelectedOrder(res);
            selectedOrderRef.current = res; // ✅ cập nhật ref ngay
            setSelectedOrderId(id);
            //setActiveTab("2");
        } catch {
            message.error("Không tải được chi tiết phiếu");
        } finally {
            setLoadingDetail(false);
        }
    };

    const refreshDetail = async () => {
        if (!selectedOrderId) return;
        try {
            const res = await getHandover(selectedOrderId);
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
        setCreateToDept(undefined);
        setCreateFromDept(undefined);
        setCreateFromLoc(undefined);
        setCreateAssetKeyword("");
        setCreateAssets([]);
        setCreateAssetsTotal(0);
        setCreateAssetsPage(1);
        setSelectedCreateNos([]);
        setFromLocOptions([]);
        setToLocOptions([]);
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

    const handleCreateFromDeptChange = async (v: string | undefined) => {
        setCreateFromDept(v);
        setCreateFromLoc(undefined);
        formCreate.setFieldValue("fromStorageLocationNo", undefined);
        setSelectedCreateNos([]);
        setCreateAssetsPage(1);
        setFromLocOptions(await loadLocOptionsByDept(v));
        loadCreateAssets(v, undefined, createAssetKeyword, 1);
    };

    const handleCreateToDeptChange = async (v: string | undefined) => {
        setCreateToDept(v);
        formCreate.setFieldValue("toStorageLocationNo", undefined);
        setToLocOptions(await loadLocOptionsByDept(v));
    };

    const handleCreateLocChange = (v: string | undefined) => {
        setCreateFromLoc(v);
        setSelectedCreateNos([]);
        setCreateAssetsPage(1);
        loadCreateAssets(createFromDept, v, createAssetKeyword, 1);
    };

    const handleCreateSubmit = async () => {
        if (!selectedCreateNos.length) {
            message.warning("Chọn ít nhất 1 tài sản để yêu cầu bàn giao");
            return;
        }
        try {
            const values = await formCreate.validateFields();
            setSaving(true);
            const created = await createHandover({
                fromDepartmentId: values.fromDepartmentId,
                toDepartmentId: values.toDepartmentId,
                fromStorageLocationNo: values.fromStorageLocationNo,
                toStorageLocationNo: values.toStorageLocationNo,
                note: values.note,
                assetNos: selectedCreateNos,
            } as CreateHandoverDto);
            message.success("Tạo phiếu yêu cầu bàn giao thành công");
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
    const openEditModal = (order: HandoverOrderDto) => {
        setEditingId(order.id);
        formEdit.setFieldsValue({
            toDepartmentId: order.toDepartmentId,
            fromStorageLocationNo: order.fromStorageLocationNo,
            toStorageLocationNo: order.toStorageLocationNo,
            note: order.note,
        });
        setEditOpen(true);
    };

    const handleEditSubmit = async (values: any) => {
        if (!editingId) return;
        setSavingEdit(true);
        try {
            await updateHandover(editingId, values as UpdateHandoverDto);
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
            await confirmHandover(selectedOrder.id);
            message.success("Xác nhận bàn giao thành công");
            setConfirmOpen(false);
            await Promise.all([loadOrders(pageIndex), refreshDetail()]);
        } catch (e: any) {
            message.error(e?.response?.data?.message ?? "Xác nhận thất bại");
        }
    };

    const handleCancel = async () => {
        if (!selectedOrder) return;
        try {
            await cancelHandover(selectedOrder.id);
            message.success("Đã hủy phiếu bàn giao");
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
        setCandidateFrom(selectedOrder?.fromDepartmentId ?? "");
        setCandidateLocation(selectedOrder?.fromStorageLocationNo ?? undefined);
        setCandidateAssetNo("");
        setCandidates([]);
        setCandidateTotal(0);
        setCandidatePage(1);
        setSelectedAssetNos([]);
        setAddAssetOpen(true);
    };

    const loadCandidates = async (page = 1) => {
        if (!candidateFrom) return;
        setLoadingCandidates(true);
        try {
            const res = await searchCandidateAssets({
                departmentId: candidateFrom,
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
            await addHandoverAssets(selectedOrder.id, { assetNos: selectedAssetNos });
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
            await removeHandoverAsset(selectedOrder.id, detailId);
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
    }); // không deps → luôn cập nhật sau mỗi render

    /* ════════════════════════════════════════════════════════════
       TOOLBAR COMMAND — đọc qua ref, không bao giờ stale
    ════════════════════════════════════════════════════════════ */
    useEffect(() => {
        if (!command) return;
        const order = selectedOrderRef.current;
        const rawStatus = order?.status as unknown as string; 
        const status: HandoverStatus | undefined = rawStatus && isNaN(Number(rawStatus))
            ? (HandoverStatus[rawStatus as keyof typeof HandoverStatus] as unknown as HandoverStatus)
            : (rawStatus as unknown as HandoverStatus);
        //const status = order?.status;
        const isEditable = status === HandoverStatus.Draft || status === HandoverStatus.Pending;
        const isConfirmable = status === HandoverStatus.Pending;
        const isCancellable = status === HandoverStatus.Draft || status === HandoverStatus.Pending;
        console.log(order);
        console.log(status + ':' + isEditable);
        switch (command) {
            case "QUERY":
                loadOrdersRef.current(1);
                break;
            case "INSERT":
                openCreateModalRef.current();
                break;
            case "MODIFY":
                if (order && isEditable) openEditModalRef.current(order);
                else message.warning("Chọn phiếu Nháp hoặc Chờ xác nhận để sửa");
                break;
            case "CONFIRM":
                if (order && isConfirmable) confirmOpenRef.current(true);
                else message.warning("Chọn phiếu Chờ xác nhận để xác nhận bàn giao");
                break;
            case "DELETE":
                if (order && isCancellable) cancelOpenRef.current(true);
                else message.warning("Chọn phiếu Nháp hoặc Chờ xác nhận để hủy");
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
    const statusOptions = Object.entries(HANDOVER_STATUS_CONFIG).map(([k, v]) => ({
        value: Number(k), label: v.label,
    }));
    const filterOption = (input: string, option?: { label: string; value: string }) =>
        (option?.label ?? "").toLowerCase().includes(input.toLowerCase());

    const orderColumns: ColumnsType<HandoverOrderListDto> = [
        {
            title: "Mã phiếu", dataIndex: "id", key: "id", width: 160, fixed: "left" as const,
            render: (id) => <a onClick={() => loadDetail(id)}>{id}</a>,
        },
        { title: "Đơn vị giao", dataIndex: "fromDepartmentName", key: "fromDept", ellipsis: true },
        { title: "Đơn vị nhận", dataIndex: "toDepartmentName", key: "toDept", ellipsis: true },
        { title: "Tài sản", dataIndex: "assetCount", key: "count", width: 80, align: "center" },
        {
            title: "Trạng thái", dataIndex: "status", key: "status", width: 140,
            render: (s: HandoverStatus) => {
                const cfg = HANDOVER_STATUS_CONFIG[s];
                return <Tag color={cfg?.color ?? "default"}>{cfg?.label ?? `(${s})`}</Tag>;
            },
        },
        {
            title: "Hết hạn", dataIndex: "createdAt", key: "expiredAt", width: 120,
            render: (createdAt: string, row) => {
                if (row.status !== HandoverStatus.Pending) return "—";
                const exp = calcExpiredAt(createdAt);
                return <Tag color={isExpired(createdAt) ? "red" : "orange"}>
                    {exp.toLocaleDateString("vi-VN")}
                </Tag>;
            },
        },
        { title: "Người tạo", dataIndex: "createdByUserName", key: "creator", width: 130 },
        {
            title: "Ngày tạo", dataIndex: "createdAt", key: "createdAt", width: 110,
            render: (v: string) => new Date(v).toLocaleDateString("vi-VN"),
        },
    ];

    const detailColumns: ColumnsType<HandoverDetailDto> = [
        { title: "Mã tài sản", dataIndex: "assetNo", key: "assetNo", width: 140 },
        { title: "Tên tài sản", dataIndex: "assetName", key: "assetName", ellipsis: true },
        { title: "Model", dataIndex: "modelName", key: "model", width: 160, ellipsis: true },
        {
            title: "ĐV đang dùng", dataIndex: "previousUseDepartmentId", key: "prevDept", width: 150,
            render: (id: string) => getDeptName(id),
        },
        {
            title: "Vị trí cũ", dataIndex: "previousStorageLocationNo", key: "prevLoc", width: 140,
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

    const assetPickColumns: ColumnsType<HandoverAssetCandidateDto> = [
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
                            <Col xs={24} sm={8} md={7}>
                                <Select allowClear placeholder="Đơn vị giao" style={{ width: "100%" }}
                                    options={deptOptions} showSearch filterOption={filterOption}
                                    onChange={(v) => setFilterFrom(v)} />
                            </Col>
                            <Col xs={24} sm={8} md={7}>
                                <Select allowClear placeholder="Đơn vị nhận" style={{ width: "100%" }}
                                    options={deptOptions} showSearch filterOption={filterOption}
                                    onChange={(v) => setFilterTo(v)} />
                            </Col>
                            <Col xs={24} sm={8} md={6}>
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
                                        <Tag color={HANDOVER_STATUS_CONFIG[selectedOrder.status]?.color ?? "default"}>
                                            {HANDOVER_STATUS_CONFIG[selectedOrder.status]?.label ?? `(${selectedOrder.status})`}
                                        </Tag>
                                    </Col>
                                    <Col xs={12} md={4}>
                                        <Text type="secondary">Đơn vị giao</Text>
                                        <br /><Text>{selectedOrder.fromDepartmentName}</Text>
                                    </Col>
                                    <Col xs={12} md={4}>
                                        <Text type="secondary">Đơn vị nhận</Text>
                                        <br /><Text>{selectedOrder.toDepartmentName}</Text>
                                    </Col>
                                    {selectedOrder.fromStorageLocationNo && (
                                        <Col xs={12} md={4}>
                                            <Text type="secondary">Vị trí giao</Text>
                                            <br /><Text>{getLocName(selectedOrder.fromStorageLocationNo)}</Text>
                                        </Col>
                                    )}
                                    {selectedOrder.toStorageLocationNo && (
                                        <Col xs={12} md={4}>
                                            <Text type="secondary">Vị trí nhận</Text>
                                            <br /><Text>{getLocName(selectedOrder.toStorageLocationNo)}</Text>
                                        </Col>
                                    )}
                                    {selectedOrder.status === HandoverStatus.Pending && (
                                        <Col xs={12} md={4}>
                                            <Text type="secondary">Hạn xác nhận</Text>
                                            <br />
                                            <Tag color={isExpired(selectedOrder.createdAt) ? "red" : "orange"}>
                                                {calcExpiredAt(selectedOrder.createdAt).toLocaleDateString("vi-VN")}
                                            </Tag>
                                        </Col>
                                    )}
                                    {selectedOrder.confirmedAt && (
                                        <Col xs={12} md={4}>
                                            <Text type="secondary">Ngày bàn giao</Text>
                                            <br /><Text>{new Date(selectedOrder.confirmedAt).toLocaleDateString("vi-VN")}</Text>
                                        </Col>
                                    )}
                                    <Col xs={24} md={8}>
                                        <Text type="secondary">Ghi chú</Text>
                                        <br /><Text>{selectedOrder.note ?? "—"}</Text>
                                    </Col>
                                    <Col xs={12} md={4}>
                                        <Text type="secondary">Người tạo (bên nhận)</Text>
                                        <br /><Text>{selectedOrder.createdByUserName}</Text>
                                    </Col>
                                    {selectedOrder.confirmedByUserName && (
                                        <Col xs={12} md={4}>
                                            <Text type="secondary">Người xác nhận (bên giao)</Text>
                                            <br /><Text>{selectedOrder.confirmedByUserName}</Text>
                                        </Col>
                                    )}
                                </Row>

                                {canEdit && (
                                    <div style={{ marginTop: 12 }}>
                                        <Button icon={<PlusOutlined />} onClick={openAddAssetModal}>
                                            Thêm tài sản
                                        </Button>
                                    </div>
                                )}
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
            <Title level={4} style={{ marginBottom: 16 }}>📦 Bàn giao tài sản</Title>

            <Tabs activeKey={activeTab} onChange={setActiveTab}
                items={tabItems} size={isMobile ? "small" : "middle"} />

            {/* ══ MODAL TẠO PHIẾU ══ */}
            <Modal title="Tạo phiếu yêu cầu bàn giao" open={createOpen}
                onCancel={() => setCreateOpen(false)} onOk={handleCreateSubmit}
                okText={`Tạo phiếu${selectedCreateNos.length ? ` (${selectedCreateNos.length} tài sản)` : ""}`}
                okButtonProps={{ disabled: !selectedCreateNos.length }}
                confirmLoading={saving} width={isMobile ? "98vw" : 900} destroyOnClose>
                <Form form={formCreate} layout="vertical">
                    <Row gutter={12}>
                        <Col xs={24} sm={12}>
                            <Form.Item name="toDepartmentId" label="Đơn vị nhận (bên tạo yêu cầu)"
                                rules={[{ required: true, message: "Chọn đơn vị nhận" }]}>
                                <Select options={deptOptions} placeholder="Chọn đơn vị nhận" showSearch
                                    filterOption={filterOption} onChange={(v) => handleCreateToDeptChange(v)} />
                            </Form.Item>
                        </Col>
                        <Col xs={24} sm={12}>
                            <Form.Item name="fromDepartmentId" label="Đơn vị giao (bên cần xác nhận)"
                                rules={[{ required: true, message: "Chọn đơn vị giao" }]}>
                                <Select options={deptOptions} placeholder="Chọn đơn vị giao" showSearch
                                    filterOption={filterOption} onChange={(v) => handleCreateFromDeptChange(v)} />
                            </Form.Item>
                        </Col>
                        <Col xs={24} sm={12}>
                            <Form.Item name="toStorageLocationNo" label="Vị trí nhận (tuỳ chọn)">
                                <Select allowClear disabled={!createToDept} options={toLocOptions}
                                    placeholder={createToDept ? "Chọn vị trí" : "Chọn đơn vị nhận trước"}
                                    showSearch filterOption={filterOption} />
                            </Form.Item>
                        </Col>
                        <Col xs={24} sm={12}>
                            <Form.Item name="fromStorageLocationNo" label="Vị trí giao (tuỳ chọn)">
                                <Select allowClear disabled={!createFromDept} options={fromLocOptions}
                                    placeholder={createFromDept ? "Chọn vị trí" : "Chọn đơn vị giao trước"}
                                    showSearch filterOption={filterOption}
                                    onChange={(v) => handleCreateLocChange(v)} />
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
                    Chọn tài sản cần bàn giao
                    {selectedCreateNos.length > 0 && (
                        <Tag color="blue" style={{ marginLeft: 8 }}>Đã chọn: {selectedCreateNos.length}</Tag>
                    )}
                </Divider>

                {!createFromDept ? (
                    <div style={{ textAlign: "center", color: "#999", padding: "16px 0" }}>
                        Chọn đơn vị giao để xem danh sách tài sản cần bàn giao
                    </div>
                ) : (
                    <>
                        <Row gutter={[8, 8]} style={{ marginBottom: 8 }}>
                            <Col xs={24} sm={16}>
                                <Input placeholder="Tìm theo mã tài sản" value={createAssetKeyword}
                                    onChange={(e) => setCreateAssetKeyword(e.target.value)}
                                    onPressEnter={() => { setCreateAssetsPage(1); loadCreateAssets(createFromDept, createFromLoc, createAssetKeyword, 1); }}
                                    allowClear />
                            </Col>
                            <Col xs={24} sm={8}>
                                <Button icon={<SearchOutlined />} block
                                    onClick={() => { setCreateAssetsPage(1); loadCreateAssets(createFromDept, createFromLoc, createAssetKeyword, 1); }}>
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
                                onChange: (p) => loadCreateAssets(createFromDept, createFromLoc, createAssetKeyword, p),
                                showSizeChanger: false, showTotal: (t) => `${t} tài sản`,
                            }} />
                    </>
                )}
            </Modal>

            {/* ══ MODAL SỬA ══ */}
            <Modal title="Sửa phiếu bàn giao" open={editOpen}
                onCancel={() => setEditOpen(false)} onOk={() => formEdit.submit()}
                okText="Lưu" confirmLoading={savingEdit}
                width={isMobile ? "95vw" : 560} destroyOnClose>
                <Form form={formEdit} layout="vertical" onFinish={handleEditSubmit}>
                    <Form.Item name="toDepartmentId" label="Đơn vị nhận"
                        rules={[{ required: true, message: "Chọn đơn vị nhận" }]}>
                        <Select options={deptOptions} showSearch filterOption={filterOption} />
                    </Form.Item>
                    <Row gutter={12}>
                        <Col span={12}>
                            <Form.Item name="fromStorageLocationNo" label="Vị trí giao">
                                <Select allowClear options={allLocOptions} showSearch filterOption={filterOption} />
                            </Form.Item>
                        </Col>
                        <Col span={12}>
                            <Form.Item name="toStorageLocationNo" label="Vị trí nhận">
                                <Select allowClear options={allLocOptions} showSearch filterOption={filterOption} />
                            </Form.Item>
                        </Col>
                    </Row>
                    <Form.Item name="note" label="Ghi chú">
                        <Input.TextArea rows={2} />
                    </Form.Item>
                </Form>
            </Modal>

            {/* ══ MODAL XÁC NHẬN ══ */}
            <Modal title="Xác nhận bàn giao?" open={confirmOpen}
                onOk={handleConfirm} onCancel={() => setConfirmOpen(false)} okText="Xác nhận bàn giao">
                <p>
                    Bên giao <strong>{selectedOrder?.fromDepartmentName}</strong> xác nhận bàn giao{" "}
                    <strong>{selectedOrder?.details?.length ?? 0}</strong> tài sản sang{" "}
                    <strong>{selectedOrder?.toDepartmentName}</strong>.
                </p>
                <p>Ngày xác nhận sẽ là ngày bàn giao chính thức. Không thể hoàn tác.</p>
            </Modal>

            {/* ══ MODAL HỦY ══ */}
            <Modal title="Hủy phiếu bàn giao?" open={cancelOpen}
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
                <Row gutter={[8, 8]} style={{ marginBottom: 12 }}>
                    <Col xs={24} sm={10}>
                        <Select style={{ width: "100%" }} value={candidateFrom || undefined}
                            placeholder="Đơn vị" options={deptOptions} showSearch
                            filterOption={filterOption} onChange={(v) => setCandidateFrom(v)} />
                    </Col>
                    <Col xs={24} sm={8}>
                        <Select allowClear style={{ width: "100%" }} value={candidateLocation}
                            placeholder="Vị trí lưu trữ" options={allLocOptions} showSearch
                            filterOption={filterOption} onChange={(v) => setCandidateLocation(v)} />
                    </Col>
                    <Col xs={24} sm={6}>
                        <Input placeholder="Mã tài sản" value={candidateAssetNo}
                            onChange={(e) => setCandidateAssetNo(e.target.value)}
                            onPressEnter={() => loadCandidates(1)} />
                    </Col>
                    <Col xs={24}>
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

export default AssetHandoverPage;