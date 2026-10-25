import { useEffect, useState, useMemo, useRef } from "react";
//import { debounce } from "lodash";
import {
    Card, Col, DatePicker, Input, Row, Select, Statistic,
    Table, Tabs, Tag, Typography, message,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import dayjs, { Dayjs } from "dayjs";
import { getDepartmentOptions, type DepartmentOptionDto } from "../../api/department.api";
import {
    getMaintenanceHistory, getMaintenanceSummaryByAsset, getMaintenanceOverallStatistic,
    RepairStatus, REPAIR_STATUS_CONFIG,
    type RepairHistoryItemDto, type AssetMaintenanceSummaryDto,
    type MaintenanceOverallStatisticDto,
} from "../../api/repair.api";

const { Title } = Typography;
const { RangePicker } = DatePicker;
const PAGE_SIZE = 20;

const AssetMaintenancePage: React.FC = () => {
    const [departments, setDepartments] = useState<DepartmentOptionDto[]>([]);
    const [activeTab, setActiveTab] = useState("1");

    /* ── bộ lọc chung ── */
    const [filterAssetNo, setFilterAssetNo] = useState("");
    const [debouncedAssetNo, setDebouncedAssetNo] = useState("");

    //const debouncedSetAssetNo = useMemo(
    //    () => debounce((value: string) => setDebouncedAssetNo(value), 400),
    //    []
    //);
    const debouncedSetAssetNo = useMemo(() => {
        let timer: ReturnType<typeof setTimeout>;
        return (value: string) => {
            clearTimeout(timer);
            timer = setTimeout(() => setDebouncedAssetNo(value), 400);
        };
    }, []);

    const handleAssetNoChange = (value: string) => {
        setFilterAssetNo(value);
        debouncedSetAssetNo(value);
    };
    const [filterDept, setFilterDept] = useState<string | undefined>();
    const [filterStatus, setFilterStatus] = useState<RepairStatus | undefined>();
    const [filterRange, setFilterRange] = useState<[Dayjs | null, Dayjs | null] | null>(null);

    /* ── tab lịch sử ── */
    const [history, setHistory] = useState<RepairHistoryItemDto[]>([]);
    const [historyTotal, setHistoryTotal] = useState(0);
    const [historyPage, setHistoryPage] = useState(1);
    const [loadingHistory, setLoadingHistory] = useState(false);

    /* ── tab thống kê theo tài sản ── */
    const [summary, setSummary] = useState<AssetMaintenanceSummaryDto[]>([]);
    const [summaryTotal, setSummaryTotal] = useState(0);
    const [summaryPage, setSummaryPage] = useState(1);
    const [loadingSummary, setLoadingSummary] = useState(false);

    /* ── tổng quan (dashboard) ── */
    const [overall, setOverall] = useState<MaintenanceOverallStatisticDto | null>(null);

        useEffect(() => {
        getDepartmentOptions()
            .then((res) => setDepartments(res.data))
            .catch(() => { });
    }, []);

    const buildQuery = () => ({
        assetNo: filterAssetNo || undefined,
        departmentId: filterDept,
        status: filterStatus,
        fromDate: filterRange?.[0] ? filterRange[0].startOf("day").toISOString() : undefined,
        toDate: filterRange?.[1] ? filterRange[1].endOf("day").toISOString() : undefined,
    });

    const loadHistory = async (page = 1) => {
        setLoadingHistory(true);
        try {
            const res = await getMaintenanceHistory({ ...buildQuery(), pageIndex: page, pageSize: PAGE_SIZE });
            setHistory(res.items);
            setHistoryTotal(res.total);
            setHistoryPage(page);
        } catch (e: any) {
            message.error(e?.response?.data?.message || "Không tải được lịch sử sửa chữa");
        } finally {
            setLoadingHistory(false);
        }
    };

    const loadSummary = async (page = 1) => {
        setLoadingSummary(true);
        try {
            const res = await getMaintenanceSummaryByAsset({ ...buildQuery(), pageIndex: page, pageSize: PAGE_SIZE });
            setSummary(res.items);
            setSummaryTotal(res.total);
            setSummaryPage(page);
        } catch (e: any) {
            message.error(e?.response?.data?.message || "Không tải được thống kê chi phí");
        } finally {
            setLoadingSummary(false);
        }
    };

    const loadOverall = async () => {
        try {
            const q = buildQuery();
            const res = await getMaintenanceOverallStatistic(q.fromDate, q.toDate, 10);
            setOverall(res);
        } catch {
            // bỏ qua lỗi thống kê tổng quan
        }
    };

    useEffect(() => {
        if (activeTab === "1") loadHistory(1);
        else loadSummary(1);
        loadOverall();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [activeTab, filterAssetNo, filterDept, filterStatus, filterRange]);

    const historyColumns: ColumnsType<RepairHistoryItemDto> = [
        { title: "Mã đơn", dataIndex: "id", width: 130 },
        { title: "Mã tài sản", dataIndex: "assetNo", width: 120 },
        { title: "Tên tài sản", dataIndex: "assetName" },
        { title: "Đơn vị", dataIndex: "departmentName", width: 150 },
        { title: "Nhà cung cấp", dataIndex: "repairVendor", width: 150 },
        { title: "Lý do", dataIndex: "reason" },
        {
            title: "Chi phí", dataIndex: "cost", width: 130, align: "right",
            render: (v?: number) => (v != null ? v.toLocaleString("vi-VN") + " đ" : "-"),
        },
        {
            title: "Trạng thái", dataIndex: "status", width: 130,
            render: (s: RepairStatus) => <Tag color={REPAIR_STATUS_CONFIG[s]?.color}>{REPAIR_STATUS_CONFIG[s]?.label}</Tag>,
        },
        {
            title: "Ngày tạo", dataIndex: "createdAt", width: 130,
            render: (v: string) => dayjs(v).format("DD/MM/YYYY"),
        },
        {
            title: "Ngày hoàn thành", dataIndex: "completedAt", width: 130,
            render: (v?: string) => (v ? dayjs(v).format("DD/MM/YYYY") : "-"),
        },
    ];

    const summaryColumns: ColumnsType<AssetMaintenanceSummaryDto> = [
        { title: "Mã tài sản", dataIndex: "assetNo", width: 120 },
        { title: "Tên tài sản", dataIndex: "assetName" },
        { title: "Model", dataIndex: "modelName", width: 150 },
        { title: "Đơn vị", dataIndex: "departmentName", width: 150 },
        { title: "Số lần sửa chữa", dataIndex: "totalRepairCount", width: 130, align: "center" },
        { title: "Số lần hoàn thành", dataIndex: "completedRepairCount", width: 130, align: "center" },
        {
            title: "Tổng chi phí", dataIndex: "totalCost", width: 150, align: "right",
            render: (v: number) => v.toLocaleString("vi-VN") + " đ",
        },
        {
            title: "Lần sửa gần nhất", dataIndex: "lastRepairAt", width: 140,
            render: (v?: string) => (v ? dayjs(v).format("DD/MM/YYYY") : "-"),
        },
    ];

    return (
        <div style={{ padding: 16 }}>
            <Title level={4}>Bảo trì máy - Lịch sử &amp; thống kê chi phí sửa chữa</Title>

            {overall && (
                <Row gutter={16} style={{ marginBottom: 16 }}>
                    <Col span={6}>
                        <Card><Statistic title="Tổng số đơn sửa chữa" value={overall.totalRepairOrders} /></Card>
                    </Col>
                    <Col span={6}>
                        <Card><Statistic title="Đang sửa chữa" value={overall.inProgressRepairOrders} /></Card>
                    </Col>
                    <Col span={6}>
                        <Card><Statistic title="Đã hoàn thành" value={overall.completedRepairOrders} /></Card>
                    </Col>
                    <Col span={6}>
                        <Card>
                            <Statistic
                                title="Tổng chi phí sửa chữa"
                                value={overall.totalCost}
                                formatter={(v) => `${Number(v).toLocaleString("vi-VN")} đ`}
                            />
                        </Card>
                    </Col>
                </Row>
            )}

            <Card style={{ marginBottom: 16 }}>
                <Row gutter={12}>
                    <Col span={5}>
                        <Input
                            placeholder="Tìm theo mã tài sản"
                            value={filterAssetNo}
                            onChange={(e) => setFilterAssetNo(e.target.value)}
                            allowClear
                        />
                    </Col>
                    <Col span={5}>
                        <Select
                            placeholder="Đơn vị"
                            allowClear
                            style={{ width: "100%" }}
                            value={filterDept}
                            onChange={setFilterDept}
                            options={departments.map((d) => ({ label: d.name, value: d.id }))}
                        />
                    </Col>
                    <Col span={5}>
                        <Select
                            placeholder="Trạng thái"
                            allowClear
                            style={{ width: "100%" }}
                            value={filterStatus}
                            onChange={setFilterStatus}
                            options={Object.entries(REPAIR_STATUS_CONFIG).map(([k, v]) => ({
                                label: v.label,
                                value: Number(k),
                            }))}
                        />
                    </Col>
                    <Col span={9}>
                        <RangePicker
                            style={{ width: "100%" }}
                            value={filterRange as any}
                            onChange={(v) => setFilterRange(v as any)}
                            format="DD/MM/YYYY"
                        />
                    </Col>
                </Row>
            </Card>

            <Tabs
                activeKey={activeTab}
                onChange={setActiveTab}
                items={[
                    {
                        key: "1",
                        label: "Lịch sử sửa chữa",
                        children: (
                            <Table<RepairHistoryItemDto>
                                rowKey="id"
                                columns={historyColumns}
                                dataSource={history}
                                loading={loadingHistory}
                                pagination={{
                                    current: historyPage,
                                    pageSize: PAGE_SIZE,
                                    total: historyTotal,
                                    onChange: (p) => loadHistory(p),
                                }}
                                scroll={{ x: 1200 }}
                            />
                        ),
                    },
                    {
                        key: "2",
                        label: "Thống kê chi phí theo tài sản",
                        children: (
                            <Table<AssetMaintenanceSummaryDto>
                                rowKey="assetNo"
                                columns={summaryColumns}
                                dataSource={summary}
                                loading={loadingSummary}
                                pagination={{
                                    current: summaryPage,
                                    pageSize: PAGE_SIZE,
                                    total: summaryTotal,
                                    onChange: (p) => loadSummary(p),
                                }}
                                scroll={{ x: 1100 }}
                            />
                        ),
                    },
                ]}
            />
        </div>
    );
};

export default AssetMaintenancePage;