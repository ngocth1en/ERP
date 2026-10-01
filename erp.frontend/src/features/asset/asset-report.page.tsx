import { useEffect, useState } from "react";
import {
    Card, Col, Row, Select, Table, Typography, message, InputNumber,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import { useUiCommandStore } from "../../stores/ui-command.store";
import { getDepartmentOptions, type DepartmentOptionDto } from "../../api/department.api";
import { getAssetBigTypes } from "../../api/asset-big-type.api";
import { getMiddleTypes } from "../../api/asset-middle-type.api";
import { getSmallTypes } from "../../api/asset-small-type.api";
import {
    searchAssetReport, exportAssetReport,
    ReportPeriodType,
    type AssetReportSearchDto, type AssetReportByDepartmentDto,
} from "../../api/asset-report.api";

const { Title } = Typography;

const AssetReportPage: React.FC = () => {
    /* ── toolbar dùng chung ── */
    const command = useUiCommandStore((s) => s.command);
    const clearCommand = useUiCommandStore((s) => s.clear);

    const [departments, setDepartments] = useState<DepartmentOptionDto[]>([]);
    const [bigTypes, setBigTypes] = useState<any[]>([]);
    const [middleTypes, setMiddleTypes] = useState<any[]>([]);
    const [smallTypes, setSmallTypes] = useState<any[]>([]);

    const [periodType, setPeriodType] = useState<ReportPeriodType>(ReportPeriodType.Month);
    const [year, setYear] = useState<number>(new Date().getFullYear());
    const [month, setMonth] = useState<number | undefined>(new Date().getMonth() + 1);
    const [quarter, setQuarter] = useState<number | undefined>(undefined);
    const [bigTypeId, setBigTypeId] = useState<number | undefined>();
    const [middleTypeId, setMiddleTypeId] = useState<number | undefined>();
    const [smallTypeId, setSmallTypeId] = useState<number | undefined>();
    const [departmentId, setDepartmentId] = useState<string | undefined>();

    const [data, setData] = useState<AssetReportByDepartmentDto[]>([]);
    const [periodLabel, setPeriodLabel] = useState("");
    const [loading, setLoading] = useState(false);
    const [exporting, setExporting] = useState(false);

    useEffect(() => {
        getDepartmentOptions().then((r) => setDepartments(r.data)).catch(() => {});
        getAssetBigTypes({ pageIndex: 1, pageSize: 100 } as any)
            .then((r: any) => setBigTypes(r.items ?? r.data?.items ?? []))
            .catch(() => {});
    }, []);

    useEffect(() => {
        if (!bigTypeId) { setMiddleTypes([]); setMiddleTypeId(undefined); return; }
        getMiddleTypes(bigTypeId).then((r: any) => setMiddleTypes(r.data ?? [])).catch(() => {});
    }, [bigTypeId]);

    useEffect(() => {
        if (!middleTypeId) { setSmallTypes([]); setSmallTypeId(undefined); return; }
        getSmallTypes(middleTypeId).then((r: any) => setSmallTypes(r.data ?? [])).catch(() => {});
    }, [middleTypeId]);

    const buildDto = (): AssetReportSearchDto => ({
        periodType,
        year,
        month: periodType === ReportPeriodType.Month ? month : undefined,
        quarter: periodType === ReportPeriodType.Quarter ? quarter : undefined,
        bigTypeId, middleTypeId, smallTypeId, departmentId,
    });

    const handleQuery = async () => {
        setLoading(true);
        try {
            const res = await searchAssetReport(buildDto());
            setData(res.data.items);
            setPeriodLabel(res.data.periodLabel);
        } catch (e: any) {
            message.error(e?.response?.data?.message || "Không tải được báo cáo");
        } finally {
            setLoading(false);
        }
    };

    const handleExport = async () => {
        setExporting(true);
        try {
            await exportAssetReport(buildDto());
            message.success("Xuất Excel thành công");
        } catch (e: any) {
            message.error(e?.response?.data?.message || "Xuất Excel thất bại");
        } finally {
            setExporting(false);
        }
    };

    /* ── lắng nghe lệnh từ toolbar dùng chung (QUERY / EXCEL) ── */
    useEffect(() => {
        if (!command) return;

        switch (command) {
            case "QUERY":
                handleQuery();
                break;

            case "EXCEL":
                handleExport();
                break;
        }

        clearCommand();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [command]);

    // Tải báo cáo mặc định khi vào trang
    useEffect(() => { handleQuery(); /* eslint-disable-next-line */ }, []);

    const columns: ColumnsType<AssetReportByDepartmentDto> = [
        { title: "Đơn vị", dataIndex: "departmentName" },
        { title: "Không sử dụng", dataIndex: "notUsedCount", align: "center" },
        { title: "Chờ sử dụng", dataIndex: "waitingCount", align: "center" },
        { title: "Đang sử dụng", dataIndex: "inUseCount", align: "center" },
        { title: "Phế", dataIndex: "scrapCount", align: "center" },
        { title: "Thanh lý", dataIndex: "liquidatedCount", align: "center" },
        { title: "Tổng cộng", dataIndex: "totalCount", align: "center", render: (v) => <b>{v}</b> },
    ];

    return (
        <div style={{ padding: 16 }}>
            <Title level={4}>Báo cáo thống kê tài sản thiết bị</Title>

            <Card style={{ marginBottom: 16 }} loading={loading || exporting}>
                <Row gutter={12}>
                    <Col span={4}>
                        <Select
                            style={{ width: "100%" }}
                            value={periodType}
                            onChange={setPeriodType}
                            options={[
                                { label: "Theo tháng", value: ReportPeriodType.Month },
                                { label: "Theo quý", value: ReportPeriodType.Quarter },
                                { label: "Theo năm", value: ReportPeriodType.Year },
                            ]}
                        />
                    </Col>
                    <Col span={3}>
                        <InputNumber style={{ width: "100%" }} min={2000} max={2100} value={year}
                            onChange={(v) => setYear(v ?? new Date().getFullYear())} placeholder="Năm" />
                    </Col>
                    {periodType === ReportPeriodType.Month && (
                        <Col span={3}>
                            <Select style={{ width: "100%" }} value={month} onChange={setMonth}
                                placeholder="Tháng"
                                options={Array.from({ length: 12 }, (_, i) => ({ label: `Tháng ${i + 1}`, value: i + 1 }))} />
                        </Col>
                    )}
                    {periodType === ReportPeriodType.Quarter && (
                        <Col span={3}>
                            <Select style={{ width: "100%" }} value={quarter} onChange={setQuarter}
                                placeholder="Quý"
                                options={[1, 2, 3, 4].map((q) => ({ label: `Quý ${q}`, value: q }))} />
                        </Col>
                    )}
                    <Col span={4}>
                        <Select allowClear style={{ width: "100%" }} placeholder="Loại lớn"
                            value={bigTypeId} onChange={setBigTypeId}
                            options={bigTypes.map((b) => ({ label: b.bigTypeNameVN, value: b.bigTypeId }))} />
                    </Col>
                    <Col span={4}>
                        <Select allowClear style={{ width: "100%" }} placeholder="Loại vừa"
                            value={middleTypeId} onChange={setMiddleTypeId} disabled={!bigTypeId}
                            options={middleTypes.map((m: any) => ({ label: m.name ?? m.middleTypeNameVN, value: m.id ?? m.middleTypeId }))} />
                    </Col>
                    <Col span={4}>
                        <Select allowClear style={{ width: "100%" }} placeholder="Loại nhỏ"
                            value={smallTypeId} onChange={setSmallTypeId} disabled={!middleTypeId}
                            options={smallTypes.map((s: any) => ({ label: s.name ?? s.smallTypeNameVN, value: s.id ?? s.smallTypeId }))} />
                    </Col>
                    <Col span={4} style={{ marginTop: 12 }}>
                        <Select allowClear style={{ width: "100%" }} placeholder="Đơn vị"
                            value={departmentId} onChange={setDepartmentId}
                            options={departments.map((d) => ({ label: d.name, value: d.id }))} />
                    </Col>
                </Row>
            </Card>

            <Card title={periodLabel}>
                <Table<AssetReportByDepartmentDto>
                    rowKey="departmentId"
                    columns={columns}
                    dataSource={data}
                    loading={loading}
                    pagination={false}
                    summary={(pageData) => {
                        const sum = (key: keyof AssetReportByDepartmentDto) =>
                            pageData.reduce((acc, cur) => acc + (cur[key] as number), 0);
                        return (
                            <Table.Summary.Row>
                                <Table.Summary.Cell index={0}><b>TỔNG CỘNG</b></Table.Summary.Cell>
                                <Table.Summary.Cell index={1} align="center">{sum("notUsedCount")}</Table.Summary.Cell>
                                <Table.Summary.Cell index={2} align="center">{sum("waitingCount")}</Table.Summary.Cell>
                                <Table.Summary.Cell index={3} align="center">{sum("inUseCount")}</Table.Summary.Cell>
                                <Table.Summary.Cell index={4} align="center">{sum("scrapCount")}</Table.Summary.Cell>
                                <Table.Summary.Cell index={5} align="center">{sum("liquidatedCount")}</Table.Summary.Cell>
                                <Table.Summary.Cell index={6} align="center"><b>{sum("totalCount")}</b></Table.Summary.Cell>
                            </Table.Summary.Row>
                        );
                    }}
                />
            </Card>
        </div>
    );
};

export default AssetReportPage;