import React, { useEffect, useState, useRef } from "react";
import { Html5QrcodeScanner } from 'html5-qrcode';
import {
    Button,
    Card,
    Col,
    Divider,
    Form,
    Grid,
    Input,
    InputNumber,
    List,
    Modal,
    Row,
    Select,
    Spin,
    Table,
    Tabs,
    Typography,
    message,Tag
} from "antd";
import {
    PlusOutlined,
    CameraOutlined,
    CloseOutlined,
} from "@ant-design/icons";
import { useAuthStore } from "../../auth/auth.store";
import { useUiCommandStore } from "../../stores/ui-command.store";
import {
    createInventoryOrder,
    getInventoryOrder,
    inventoryCheck,
    searchInventoryOrders,
    updateInventoryOrder,
} from "../../api/inventory.api";
import {
    getAssetModelOptions,
    type AssetModelOptionDto,
} from "../../api/asset-model.api";
import {
    getDepartmentOptions,
    type DepartmentOptionDto,
} from "../../api/department.api";
import {
    getAllStorageLocations,
    type StorageLocationDto,
} from "../../api/location.api";

const { Text } = Typography;
const { useBreakpoint } = Grid;

type OrderRow = {
    id: string;
    modelId: number;
    departmentId?: string;
    checkLocationNo?: string;
    inventoryCheckType: string;
    inventoryType: string;
    expectedCount: number;
    counted: number;
    createdAt: string;
    createdByUserName?: string;
};

const PAGE_SIZE = 20;

// ✅ Label trạng thái tiếng Việt
const STATUS_CONFIG: Record<string, { label: string; color: string }> = {
    FOUND: { label: "Tài sản này đã kiểm ✅", color: "success" },
    UNEXPECTED: { label: "Tài sản này nằm sai vị trí ⚠️", color: "warning" },
    MISSING: { label: "Chưa kiểm", color: "error" },
    PENDING: { label: "Chưa kiểm", color: "default" },
};

const AssetInventoryPage: React.FC = () => {
    const username = useAuthStore((s) => s.username);
    const screens = useBreakpoint();
    const isMobile = !screens.md;

    /* ===== TOOLBAR COMMAND ===== */
    const command = useUiCommandStore((s) => s.command);
    const clearCommand = useUiCommandStore((s) => s.clear);

    /* ===== LOOKUP DATA ===== */
    const [models, setModels] = useState<AssetModelOptionDto[]>([]);
    const [departments, setDepartments] = useState<DepartmentOptionDto[]>([]);
    const [storageLocations, setStorageLocations] = useState<StorageLocationDto[]>([]);
    const [loadingLookup, setLoadingLookup] = useState(true);

    /* ===== FILTER STATE ===== */
    const [filters, setFilters] = useState<{
        orderId: string;
        modelId: "ALL" | number;
        departmentId: "ALL" | string;
    }>({
        orderId: "",
        modelId: "ALL",
        departmentId: "ALL",
    });

    /* ===== TAB 1 - ORDERS LIST STATE ===== */
    const [orders, setOrders] = useState<OrderRow[]>([]);
    const [pageIndex, setPageIndex] = useState(1);
    const [pageSize] = useState(PAGE_SIZE);
    const [total, setTotal] = useState(0);
    const [loadingList, setLoadingList] = useState(false);

    /* ===== TAB 2 - ORDER DETAIL STATE ===== */
    const [activeTab, setActiveTab] = useState("1");
    const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
    const [orderDetail, setOrderDetail] = useState<any | null>(null);
    const orderDetailRef = useRef<any | null>(null);
    const [loadingDetail, setLoadingDetail] = useState(false);

    /* ===== MODAL CREATE/UPDATE STATE ===== */
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingOrder, setEditingOrder] = useState<OrderRow | null>(null);
    const [form] = Form.useForm();
    const [savingOrder, setSavingOrder] = useState(false);

    /* ===== SCAN / COUNT BUFFERS ===== */
    const [scanInput, setScanInput] = useState("");
    const [scanItems, setScanItems] = useState<
        Array<{ code: string; note?: string; isDamaged?: boolean }>
    >([]);
    const [countedQty, setCountedQty] = useState<number | undefined>();
    const [performingCheck, setPerformingCheck] = useState(false);

    /* ===== CAMERA SCAN STATE ===== */
    const [showCamera, setShowCamera] = useState(false);
    const scannerRef = useRef<Html5QrcodeScanner | null>(null);
    const lastScannedRef = useRef<{ code: string; time: number } | null>(null); // ✅ thêm dòng này
    const addScanFromCameraRef = useRef<(code: string) => void>(() => { });
    useEffect(() => {
        addScanFromCameraRef.current = addScanFromCamera;
    }); // ✅ không deps → update sau mỗi render, luôn là version mới nhất
    /* ===== INIT - LOAD LOOKUPS ===== */
    useEffect(() => {
        const loadLookups = async () => {
            try {
                setLoadingLookup(true);
                const [modelsRes, deptsRes, locationsRes] = await Promise.all([
                    getAssetModelOptions(),
                    getDepartmentOptions(),
                    getAllStorageLocations(),
                ]);
                setModels(modelsRes.data);
                setDepartments(deptsRes.data);
                setStorageLocations(locationsRes.data);
            } catch (err: any) {
                message.error(err?.message || "Lỗi khi tải dữ liệu lookup");
            } finally {
                setLoadingLookup(false);
            }
        };
        loadLookups();
    }, []);

    /* ===== LOAD ORDERS WHEN FILTERS CHANGE ===== */
    useEffect(() => {
        loadOrders();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [pageIndex, filters]);

    /* ===== TOOLBAR COMMAND HANDLER ===== */
    useEffect(() => {
        if (!command) return;

        switch (command) {
            case "QUERY":
                setPageIndex(1);
                loadOrders();
                break;

            case "INSERT":
                form.resetFields();
                setEditingOrder(null);
                setIsModalOpen(true);
                break;
            //case "MODIFY":
            //    if (orderDetail) {
            //        setEditingOrder({ ... });
            //        form.setFieldsValue({ inventoryCheckType: orderDetail.inventoryCheckType });
            //        setIsModalOpen(true);
            //    }
            //    break;

            default:
                break;
        }

        clearCommand();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [command]);

    /* ===== CLEANUP CAMERA ON UNMOUNT ===== */
    useEffect(() => {
        return () => {
            if (scannerRef.current) {
                try {
                    scannerRef.current.clear();
                } catch (err) {
                    console.log("Lỗi khi đóng camera:", err);
                }
            }
        };
    }, []);
    // ✅ Helper: luôn update cả ref và state cùng lúc
    function setOrderDetailSync(data: any) {
        orderDetailRef.current = data;
        setOrderDetail(data);
    }
    /* ===== LOAD ORDERS LIST ===== */
    async function loadOrders() {
        try {
            setLoadingList(true);
            const res = await searchInventoryOrders({
                modelId: filters.modelId === "ALL" ? undefined : filters.modelId,
                departmentId:
                    filters.departmentId === "ALL" ? undefined : filters.departmentId,
                pageIndex,
                pageSize,
            });

            let filteredItems: OrderRow[] = res.items ?? [];
            if (filters.orderId.trim()) {
                const searchTerm = filters.orderId.toLowerCase();
                filteredItems = filteredItems.filter((item) =>
                    item.id.toLowerCase().includes(searchTerm)
                );
            }

            setOrders(filteredItems);
            setTotal(filteredItems.length);
        } catch (err: any) {
            message.error(err?.message || "Lỗi khi tải danh sách đơn kiểm");
        } finally {
            setLoadingList(false);
        }
    }

    /* ===== LOAD ORDER DETAIL ===== */
    async function loadOrderDetail(id: string) {
        console.log("Click ID:", id);
        try {
            setLoadingDetail(true);
            const res = await getInventoryOrder(id);
            console.log("API Result:", res);
            //setOrderDetail(res);
            setOrderDetailSync(res);
            setSelectedOrderId(id);
            setScanItems([]);
            setScanInput("");
            setCountedQty(undefined);
            setShowCamera(false);
        } catch (err: any) {
            message.error(err?.message || "Lỗi khi tải chi tiết đơn");
        } finally {
            setLoadingDetail(false);
        }
    }
    /* ===== REFRESH ORDER DETAIL — sau scan: KHÔNG reset buffer ===== */
    async function refreshOrderDetail(id: string) {
        try {
            const res = await getInventoryOrder(id);
            //setOrderDetail(res);
            setOrderDetailSync(res);// ✅ chỉ update data, giữ nguyên scanItems
        } catch (err: any) {
            message.error(err?.message || "Lỗi khi tải chi tiết đơn");
        }
    }
    /* ===== OPEN EDIT MODAL ===== */
    function openEditModal(order: OrderRow) {
        setEditingOrder(order);
        form.setFieldsValue({
            modelId: order.modelId,
            departmentId: order.departmentId,
            checkLocationNo: order.checkLocationNo,
            inventoryCheckType: order.inventoryCheckType,
            note: undefined,
        });
        setIsModalOpen(true);
    }

    /* ===== SAVE MODAL ===== */
    async function onModalOk() {
        try {
            const vals = await form.validateFields();
            setSavingOrder(true);
            if (editingOrder) {
                await updateInventoryOrder(
                    editingOrder.id,
                    {
                        departmentId: vals.departmentId,
                        checkLocationNo: vals.checkLocationNo,
                        inventoryCheckType: vals.inventoryCheckType,
                        note: vals.note,
                    },
                    username
                );
                message.success("Cập nhật đơn thành công");
                if (selectedOrderId === editingOrder.id) {
                    await loadOrderDetail(editingOrder.id);
                }
            } else {
                await createInventoryOrder(
                    {
                        modelId: vals.modelId,
                        departmentId: vals.departmentId,
                        checkLocationNo: vals.checkLocationNo,
                        inventoryType: vals.inventoryCheckType,
                        note: vals.note,
                    },
                    username
                );
                message.success("Tạo đơn thành công");
            }
            setIsModalOpen(false);
            setPageIndex(1);
            loadOrders();
        } catch (err: any) {
            message.error(err?.message || "Lỗi lưu đơn");
        } finally {
            setSavingOrder(false);
        }
    }

    /* ===== START CAMERA SCAN ===== */
    async function startCamera() {
        setShowCamera(true);

        setTimeout(() => {
            if (!scannerRef.current) {
                const readerElement = document.getElementById("qr-reader");

                if (!readerElement) {
                    message.error("Không tìm thấy element camera. Vui lòng reload trang.");
                    setShowCamera(false);
                    return;
                }

                try {
                    const scanner = new Html5QrcodeScanner(
                        "qr-reader",
                        {
                            fps: 10,
                            qrbox: { width: 250, height: 250 },
                            aspectRatio: 1.33,
                        },
                        false
                    );

                    scanner.render(
                        (decodedText) => {
                            const now = Date.now();
                            const COOLDOWN_MS = 3000;

                            if (
                                lastScannedRef.current &&
                                lastScannedRef.current.code === decodedText &&
                                now - lastScannedRef.current.time < COOLDOWN_MS
                            ) {
                                return;
                            }

                            lastScannedRef.current = { code: decodedText, time: now };
                            setScanInput(decodedText);
                            // ✅ Gọi qua ref — luôn là function mới nhất
                            addScanFromCameraRef.current(decodedText);
                        },
                        (error) => { console.warn("⚠️ Quét lỗi:", error); }
                    );

                    scannerRef.current = scanner;
                } catch (err: any) {
                    message.error("Lỗi khi khởi động camera: " + err.message);
                    setShowCamera(false);
                }
            }
        }, 100);
    }

    /* ===== STOP CAMERA SCAN ===== */
    function stopCamera() {
        if (scannerRef.current) {
            try {
                scannerRef.current.clear();
                scannerRef.current = null;
            } catch (err) {
                console.log("Lỗi khi đóng camera:", err);
            }
        }
        setShowCamera(false);
    }

    ///* ===== ADD SCAN FROM CAMERA ===== */
    //function addScanFromCamera(code: string) {
    //    setScanItems((s) => {
    //        if (s.some((i) => i.code.toLowerCase() === code.toLowerCase())) {
    //            message.warning("Mã này đã được quét");
    //            if (navigator.vibrate) navigator.vibrate(200);
    //            return s;
    //        }
    //        if (navigator.vibrate) navigator.vibrate([50, 50, 50]);
    //        return [...s, { code }];
    //    });
    //}

    ///* ===== ADD SCAN ITEM ===== */
    //function addScan() {
    //    const code = (scanInput || "").trim();
    //    if (!code) return;
    //    setScanItems((s) => {
    //        if (s.some((i) => i.code.toLowerCase() === code.toLowerCase())) {
    //            message.warning("Mã này đã được quét");
    //            if (navigator.vibrate) navigator.vibrate(200);
    //            return s;
    //        }
    //        if (navigator.vibrate) navigator.vibrate([50, 50, 50]);
    //        return [...s, { code }];
    //    });
    //    setScanInput("");
    //}

    /* ===== REMOVE SCAN ITEM ===== */
    function removeScan(code: string) {
        setScanItems((s) => s.filter((i) => i.code !== code));
    }

    ///* ===== PERFORM CHECK ===== */
    //async function performCheck() {
    //    if (!selectedOrderId || !orderDetail) {
    //        message.error("Vui lòng chọn đơn kiểm trước");
    //        return;
    //    }

    //    try {
    //        setPerformingCheck(true);

    //        const body =
    //            orderDetail.inventoryType === "2"
    //                ? {
    //                    inventoryId: selectedOrderId,
    //                    modelId: orderDetail.modelId,
    //                    departmentId: orderDetail.departmentId,
    //                    items: scanItems,
    //                }
    //                : {
    //                    inventoryId: selectedOrderId,
    //                    modelId: orderDetail.modelId,
    //                    departmentId: orderDetail.departmentId,
    //                    countedQuantity: countedQty,
    //                };

    //        const res = await inventoryCheck(body, username);
    //        message.success(
    //            `Kết quả: Found=${res.counted}, Missing=${res.missingCount ?? 0}, Unexpected=${res.unexpectedCount ?? 0}`
    //        );

    //        await loadOrderDetail(selectedOrderId);
    //        loadOrders();
    //    } catch (err: any) {
    //        message.error(err?.message || "Lỗi khi thực hiện kiểm kê");
    //    } finally {
    //        setPerformingCheck(false);
    //    }
    //}
    /* ===== ADD SCAN FROM CAMERA — gửi ngay lên BE ===== */
    async function addScanFromCamera(code: string) {
        const isDuplicate = scanItems.some(
            (i) => i.code.toLowerCase() === code.toLowerCase()
        );
        if (isDuplicate) {
            message.warning("Mã này đã được quét");
            if (navigator.vibrate) navigator.vibrate(200);
            return;
        }

        // ✅ Gửi ngay 1 mã lên BE
        try {
            await inventoryCheck(
                { inventoryId: selectedOrderId!, scannedQRCode: code },
                username
            );
            if (navigator.vibrate) navigator.vibrate([50, 50, 50]);
            // Cập nhật local list và reload detail
            setScanItems((s) => [...s, { code }]);
            await refreshOrderDetail(selectedOrderId!); 
            //await loadOrderDetail(selectedOrderId!);
        } catch (err: any) {
            message.error(err?.message || "Lỗi khi ghi nhận mã QR");
            if (navigator.vibrate) navigator.vibrate(500);
        }
    }

    /* ===== ADD SCAN ITEM (nhập tay) — gửi ngay lên BE ===== */
    async function addScan() {
        const code = (scanInput || "").trim();
        if (!code) return;
        if (scanItems.some((i) => i.code.toLowerCase() === code.toLowerCase())) {
            message.warning("Mã này đã được quét trong phiên hiện tại");
            return;
        }

        // ✅ Cùng logic check đã FOUND
        const alreadyChecked = (orderDetailRef.current?.details ?? []).some(
            (d: any) =>
                d.status === "FOUND" &&
                (d.qrCode?.toLowerCase() === code.toLowerCase() ||
                    d.assetNo?.toLowerCase() === code.toLowerCase())
        );
        if (alreadyChecked) {
            message.warning("Tài sản này đã được kiểm rồi ✅");
            if (navigator.vibrate) navigator.vibrate([100, 50, 100]);
            return;
        }
        const isDuplicate = scanItems.some(
            (i) => i.code.toLowerCase() === code.toLowerCase()
        );
        if (isDuplicate) {
            message.warning("Mã này đã được quét");
            if (navigator.vibrate) navigator.vibrate(200);
            return;
        }

        // ✅ Gửi ngay 1 mã lên BE
        try {
            await inventoryCheck(
                { inventoryId: selectedOrderId!, scannedQRCode: code },
                username
            );
            if (navigator.vibrate) navigator.vibrate([50, 50, 50]);
            setScanItems((s) => [...s, { code }]);
            setScanInput("");
            await refreshOrderDetail(selectedOrderId!); 
            //await loadOrderDetail(selectedOrderId!);
        } catch (err: any) {
            message.error(err?.message || "Lỗi khi ghi nhận mã QR");
        }
    }

    /* ===== PERFORM SCAN CHECK — scan mode: gửi scanInput nếu còn ===== */
    async function performScanCheck() {
        if (!selectedOrderId || !orderDetail) {
            message.error("Vui lòng chọn đơn kiểm trước");
            return;
        }

        // ✅ Nếu ô input còn nội dung chưa gửi → gửi luôn
        const pendingCode = (scanInput || "").trim();
        if (pendingCode) {
            const isDuplicate = scanItems.some(
                (i) => i.code.toLowerCase() === pendingCode.toLowerCase()
            );
            if (isDuplicate) {
                message.warning("Mã này đã được quét rồi");
            } else {
                try {
                    setPerformingCheck(true);
                    await inventoryCheck(
                        { inventoryId: selectedOrderId, scannedQRCode: pendingCode },
                        username
                    );
                    if (navigator.vibrate) navigator.vibrate([50, 50, 50]);
                    setScanItems((s) => [...s, { code: pendingCode }]);
                    setScanInput("");
                    const fresh = await getInventoryOrder(selectedOrderId);
                    //setOrderDetail(fresh);
                    setOrderDetailSync(fresh);
                    message.success(`Đã kiểm: ${fresh.counted} / ${fresh.expectedCount}`);
                    //await refreshOrderDetail(selectedOrderId);
                    //await loadOrderDetail(selectedOrderId);
                } catch (err: any) {
                    message.error(err?.message || "Lỗi khi ghi nhận mã QR");
                    return;
                } finally {
                    setPerformingCheck(false);
                }
            }
        } else {
            const fresh = await getInventoryOrder(selectedOrderId);
            setOrderDetail(fresh);
            message.success(`Đã kiểm: ${fresh.counted} / ${fresh.expectedCount}`);
        }
        //} else {
        //    // Không có input → chỉ reload lại kết quả
        //    await loadOrderDetail(selectedOrderId);
        //}

        //message.success(
        //    `Đã kiểm: ${orderDetail.counted} / ${orderDetail.expectedCount}`
        //);
    }

    // ✅ Thêm width + fixed cho tất cả cột
    const columns = [
        {
            title: "Mã đơn",
            dataIndex: "id",
            key: "id",
            width: 150,
            fixed: "left" as const,
        },
        {
            title: "Model",
            dataIndex: "modelId",
            key: "modelId",
            width: 150,
            render: (modelId: number) => {
                const model = models.find((m) => m.id === modelId);
                return model?.name || modelId;
            },
        },
        {
            title: "Phòng ban",
            dataIndex: "departmentId",
            key: "departmentId",
            width: 150,
            render: (deptId: string) => {
                const dept = departments.find((d) => d.id === deptId);
                return dept?.name || deptId || "-";
            },
        },
        {
            title: "Vị trí kiểm kê",
            dataIndex: "checkLocationNo",
            key: "checkLocationNo",
            width: 160,
            render: (locationNo: string) => {
                const location = storageLocations.find(
                    (l) => l.storageLocationNo === locationNo
                );
                return location?.nameVN || locationNo || "-";
            },
        },
        {
            title: "Loại",
            dataIndex: "inventoryCheckType",
            key: "inventoryCheckType",
            width: 90,
        },
        {
            title: "Dự kiến",
            dataIndex: "expectedCount",
            key: "expectedCount",
            width: 90,
        },
        {
            title: "Đã kiểm",
            dataIndex: "counted",
            key: "counted",
            width: 90,
        },
        {
            title: "Người tạo",
            dataIndex: "createdByUserName",
            key: "createdByUserName",
            width: 130,
        },
    ];

    const tabItems = [
        {
            key: "1",
            label: "Danh sách đơn kiểm",
            children: (
                <>
                    <Row gutter={[16, 12]} style={{ marginBottom: 16 }}>
                        <Col xs={24} sm={12} md={6}>
                            <Input
                                placeholder="Mã đơn"
                                value={filters.orderId}
                                onChange={(e) =>
                                    setFilters({ ...filters, orderId: e.target.value })
                                }
                            />
                        </Col>

                        <Col xs={24} sm={12} md={6}>
                            <Select
                                style={{ width: "100%" }}
                                placeholder="Model"
                                value={filters.modelId}
                                options={[
                                    { value: "ALL", label: "Tất cả" },
                                    ...models.map((x) => ({ value: x.id, label: x.name })),
                                ]}
                                onChange={(v) => setFilters({ ...filters, modelId: v })}
                                loading={loadingLookup}
                            />
                        </Col>

                        <Col xs={24} sm={12} md={6}>
                            <Select
                                style={{ width: "100%" }}
                                placeholder="Phòng ban"
                                value={filters.departmentId}
                                options={[
                                    { value: "ALL", label: "Tất cả" },
                                    ...departments.map((x) => ({ value: x.id, label: x.name })),
                                ]}
                                onChange={(v) => setFilters({ ...filters, departmentId: v })}
                                loading={loadingLookup}
                            />
                        </Col>
                    </Row>

                    {/* ✅ scroll={{ x: "max-content" }} để table scroll ngang thay vì tràn */}
                    <Table
                        rowKey="id"
                        loading={loadingList}
                        dataSource={orders}
                        columns={columns}
                        scroll={{ x: "max-content" }}
                        onRow={(record) => ({
                            onClick: () => {
                                loadOrderDetail(record.id);
                            },
                            style: {
                                cursor: "pointer",
                                background:
                                    record.id === selectedOrderId ? "#e6f4ff" : undefined,
                            },
                        })}
                        pagination={{
                            current: pageIndex,
                            pageSize,
                            total,
                            onChange: (p) => setPageIndex(p),
                            showSizeChanger: false,
                        }}
                    />
                </>
            ),
        },
        {
            key: "2",
            label: "Chi tiết kiểm kê",
            children: (
                <>
                    {!selectedOrderId && (
                        <div style={{ padding: "20px", textAlign: "center" }}>
                            <Text type="secondary">
                                Chọn một đơn từ tab "Danh sách đơn kiểm" để xem chi tiết.
                            </Text>
                        </div>
                    )}

                    {loadingDetail && selectedOrderId && (
                        <div style={{ textAlign: "center", padding: "20px" }}>
                            <Spin />
                        </div>
                    )}

                    {orderDetail && !loadingDetail && selectedOrderId && (
                        <>
                            {/* THÔNG TIN ĐƠN KIỂM */}
                            <Card style={{ marginBottom: 16 }}>
                                <Row gutter={[16, 12]}>
                                    <Col xs={12} sm={12} md={6}>
                                        <Text strong>Phiếu:</Text>
                                        <br />
                                        <Text>{orderDetail.id}</Text>
                                    </Col>
                                    <Col xs={12} sm={12} md={6}>
                                        <Text strong>Model:</Text>
                                        <br />
                                        <Text>
                                            {models.find((m) => m.id === orderDetail.modelId)?.name ||
                                                orderDetail.modelId}
                                        </Text>
                                    </Col>
                                    <Col xs={12} sm={12} md={6}>
                                        <Text strong>Vị trí:</Text>
                                        <br />
                                        <Text>
                                            {storageLocations.find(
                                                (l) => l.storageLocationNo === orderDetail.checkLocationNo
                                            )?.nameVN || orderDetail.checkLocationNo || "-"}
                                        </Text>
                                    </Col>
                                    <Col xs={12} sm={12} md={6}>
                                        <Text strong>Loại:</Text>
                                        <br />
                                        <Text>
                                            {orderDetail.inventoryType === "2"
                                                ? "QR Code"
                                                : "Đếm số lượng"}
                                        </Text>
                                    </Col>
                                    <Col xs={12} sm={12} md={6}>
                                        <Text strong>Dự kiến:</Text>
                                        <br />
                                        <Text>{orderDetail.expectedCount}</Text>
                                    </Col>
                                    <Col xs={12} sm={12} md={6}>
                                        <Text strong>Đã kiểm:</Text>
                                        <br />
                                        <Text>{orderDetail.counted}</Text>
                                    </Col>
                                </Row>
                            </Card>

                            {/* QUÉT QR CODE HOẶC NHẬP SỐ LƯỢNG */}
                            {orderDetail.inventoryType === "2" ? (
                                <Card
                                    title="Quét QR Code"
                                    style={{ marginBottom: 16, padding: 0 }}
                                >
                                    {!showCamera ? (
                                        <div style={{ padding: 16 }}>
                                            <Row gutter={[8, 8]} style={{ marginBottom: 16 }}>
                                                <Col xs={24}>
                                                    <Button
                                                        type="primary"
                                                        block
                                                        size="large"
                                                        icon={<CameraOutlined />}
                                                        onClick={startCamera}
                                                    >
                                                        📷 Mở Camera Quét
                                                    </Button>
                                                </Col>
                                            </Row>

                                            <Divider />

                                            <Row gutter={[8, 8]} style={{ marginBottom: 16 }}>
                                                <Col xs={24} sm={16}>
                                                    <Input
                                                        placeholder="Hoặc nhập/dán mã thủ công"
                                                        value={scanInput}
                                                        onChange={(e) => setScanInput(e.target.value)}
                                                        onPressEnter={addScan}
                                                        autoFocus
                                                        size="large"
                                                    />
                                                </Col>
                                                <Col xs={12} sm={4}>
                                                    <Button
                                                        onClick={addScan}
                                                        icon={<PlusOutlined />}
                                                        block
                                                        size="large"
                                                    >
                                                        Thêm
                                                    </Button>
                                                </Col>
                                                <Col xs={12} sm={4}>
                                                    <Button
                                                        type="primary"
                                                        onClick={performScanCheck}
                                                        loading={performingCheck}
                                                        block
                                                        size="large"
                                                    >
                                                        Kiểm kê
                                                    </Button>
                                                </Col>
                                            </Row>
                                        </div>
                                    ) : (
                                        <>
                                            <div
                                                id="qr-reader"
                                                style={{
                                                    width: "100%",
                                                    borderRadius: 8,
                                                    overflow: "hidden",
                                                    marginBottom: 16,
                                                }}
                                            />
                                            <div style={{ padding: "0 16px 16px" }}>
                                                <Button
                                                    danger
                                                    block
                                                    size="large"
                                                    icon={<CloseOutlined />}
                                                    onClick={stopCamera}
                                                >
                                                    ❌ Đóng Camera
                                                </Button>
                                            </div>
                                        </>
                                    )}

                                    <div
                                        style={{
                                            padding: "0 16px",
                                            marginBottom: 16,
                                            textAlign: "center",
                                        }}
                                    >
                                        <Text strong style={{ fontSize: 16 }}>
                                            ✅ Đã quét: {scanItems.length} mã
                                        </Text>
                                    </div>

                                    <List
                                        bordered
                                        dataSource={scanItems}
                                        style={{ maxHeight: 300, overflowY: "auto" }}
                                        renderItem={(item, index) => (
                                            <List.Item
                                                actions={[
                                                    <Button
                                                        danger
                                                        size="small"
                                                        type="link"
                                                        onClick={() => removeScan(item.code)}
                                                    >
                                                        Xóa
                                                    </Button>,
                                                ]}
                                            >
                                                <div style={{ width: "100%" }}>
                                                    <Text>
                                                        {index + 1}. {item.code}
                                                    </Text>
                                                    {item.note && (
                                                        <Text type="secondary"> - {item.note}</Text>
                                                    )}
                                                </div>
                                            </List.Item>
                                        )}
                                    />
                                </Card>
                            ) : (
                                <Card title="Nhập số lượng" style={{ marginBottom: 16 }}>
                                    <Form layout="vertical">
                                        <Form.Item label="Số lượng">
                                            <InputNumber
                                                value={countedQty}
                                                onChange={(v) =>
                                                    setCountedQty(v as number | undefined)
                                                }
                                                size="large"
                                                min={0}
                                                style={{ width: "100%" }}
                                            />
                                        </Form.Item>
                                        <Form.Item>
                                            <Button
                                                type="primary"
                                                onClick={performScanCheck}
                                                loading={performingCheck}
                                                block
                                                size="large"
                                            >
                                                Xác nhận
                                            </Button>
                                        </Form.Item>
                                    </Form>
                                </Card>
                            )}

                            {/* CHI TIẾT KẾT QUẢ KIỂM KÊ */}
                            <Card title="Chi tiết kiểm kê">
                                <List
                                    bordered
                                    dataSource={orderDetail.details || []}
                                    renderItem={(d: any) => (
                                        <List.Item>
                                            <div
                                                style={{
                                                    width: "100%",
                                                    display: "flex",
                                                    justifyContent: "space-between",
                                                    alignItems: "center",
                                                    flexWrap: "wrap",
                                                    gap: 8,
                                                }}
                                            >
                                                <div>
                                                    <Text strong>{d.assetNo}</Text>
                                                    <Divider type="vertical" />
                                                    {/*<Text*/}
                                                    {/*    type={*/}
                                                    {/*        d.status === "FOUND"*/}
                                                    {/*            ? "success"*/}
                                                    {/*            : d.status === "MISSING"*/}
                                                    {/*                ? "danger"*/}
                                                    {/*                : "warning"*/}
                                                    {/*    }*/}
                                                    {/*>*/}
                                                    {/*    {d.status === "PENDING"*/}
                                                    {/*        ? "Chưa kiểm kê"*/}
                                                    {/*        : d.status}*/}
                                                    {/*</Text>*/}
                                                    {(() => {
                                                        const cfg = STATUS_CONFIG[d.status] ?? STATUS_CONFIG["PENDING"];
                                                        return <Tag color={cfg.color}>{cfg.label}</Tag>;
                                                    })()}
                                                </div>
                                                <Text type="secondary">{d.note}</Text>
                                            </div>
                                        </List.Item>
                                    )}
                                />
                            </Card>
                        </>
                    )}
                </>
            ),
        },
    ];

    return (
        <>
            <Typography.Title level={4}>Quản lý Kiểm kê Tài sản</Typography.Title>

            {/* ✅ size responsive cho Tabs */}
            <Tabs
                activeKey={activeTab}
                onChange={setActiveTab}
                items={tabItems}
                size={isMobile ? "small" : "middle"}
            />

            {/* ✅ Modal width responsive */}
            <Modal
                open={isModalOpen}
                title={editingOrder ? "Sửa loại kiểm kê" : "Tạo phiếu kiểm"}
                onOk={onModalOk}
                onCancel={() => setIsModalOpen(false)}
                confirmLoading={savingOrder}
                width={isMobile ? "95vw" : 600}
            >
                <Form form={form} layout="vertical">
                    {!editingOrder ? (
                        <>
                            <Form.Item
                                name="modelId"
                                label="Model"
                                rules={[{ required: true, message: "Vui lòng chọn model" }]}
                            >
                                <Select
                                    placeholder="Chọn model"
                                    options={models.map((m) => ({
                                        value: m.id,
                                        label: m.name,
                                    }))}
                                    loading={loadingLookup}
                                />
                            </Form.Item>

                            <Form.Item name="departmentId" label="Phòng ban sử dụng">
                                <Select
                                    placeholder="Chọn phòng ban"
                                    allowClear
                                    options={departments.map((d) => ({
                                        value: d.id,
                                        label: d.name,
                                    }))}
                                    loading={loadingLookup}
                                />
                            </Form.Item>

                            <Form.Item
                                name="checkLocationNo"
                                label="Vị trí kiểm kê"
                                rules={[
                                    {
                                        required: true,
                                        message: "Vui lòng chọn vị trí kiểm kê",
                                    },
                                ]}
                            >
                                <Select
                                    placeholder="Chọn vị trí kiểm kê"
                                    options={storageLocations.map((l) => ({
                                        value: l.storageLocationNo,
                                        label: l.nameVN,
                                    }))}
                                    loading={loadingLookup}
                                />
                            </Form.Item>
                        </>
                    ) : null}

                    <Form.Item
                        name="inventoryCheckType"
                        label="Loại kiểm kê"
                        rules={[{ required: true, message: "Vui lòng chọn loại kiểm kê" }]}
                    >
                        <Select
                            placeholder="Chọn loại kiểm kê"
                            options={[
                                { value: "1", label: "Quét QR Code" },
                                { value: "2", label: "Đếm số lượng" },
                            ]}
                        />
                    </Form.Item>

                    {!editingOrder ? (
                        <Form.Item name="note" label="Ghi chú">
                            <Input.TextArea rows={3} placeholder="Nhập ghi chú" />
                        </Form.Item>
                    ) : null}
                </Form>
            </Modal>
        </>
    );
};

export default AssetInventoryPage;