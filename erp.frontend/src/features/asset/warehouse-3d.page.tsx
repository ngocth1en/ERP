import { useEffect, useState } from "react";
import { Typography, Select, Row, Col, Card, List, Tag, Empty, Spin, message } from "antd";
import {
    getWarehouseFloorPlan,
    getAssetsByStorageLocation,
    type WarehouseCellDto,
    type WarehouseCellAssetDto,
} from "../../api/location.api";
import WarehouseFloorPlan3D from "../../components/shared/WarehouseFloorPlan3D";
import Asset3DViewer from "../../components/shared/Asset3DViewer";

const { Title, Text } = Typography;

const FLOOR_OPTIONS = [1, 2, 3, 4, 5, 6].map((f) => ({
    value: f,
    label: `Tầng ${f}`,
}));

const STATUS_LABEL: Record<number, string> = {
    1: "Đang dùng",
    2: "Trống",
    3: "Báo phế",
};

const WarehouseFloorPlanPage = () => {
    const [floor, setFloor] = useState(1);
    const [cells, setCells] = useState<WarehouseCellDto[]>([]);
    const [loading, setLoading] = useState(false);

    const [selectedCell, setSelectedCell] = useState<WarehouseCellDto | undefined>();
    const [assets, setAssets] = useState<WarehouseCellAssetDto[]>([]);
    const [loadingAssets, setLoadingAssets] = useState(false);

    const [previewUrl, setPreviewUrl] = useState<string | null>(null);

    /* ===== LOAD FLOOR PLAN ===== */
    useEffect(() => {
        setLoading(true);
        setSelectedCell(undefined);
        setAssets([]);

        getWarehouseFloorPlan(floor)
            .then((res) => setCells(res.data.cells))
            .catch(() => message.error("Không tải được sơ đồ kho"))
            .finally(() => setLoading(false));
    }, [floor]);

    /* ===== CLICK CHỌN Ô (raycasting) ===== */
    const handleSelectCell = (cell: WarehouseCellDto) => {
        setSelectedCell(cell);
        setLoadingAssets(true);

        getAssetsByStorageLocation(cell.storageLocationNo)
            .then((res) => setAssets(res.data))
            .catch(() => message.error("Không tải được danh sách tài sản"))
            .finally(() => setLoadingAssets(false));
    };

    return (
        <>
            <Title level={4}>Sơ đồ kho 3D (CAD/3D Warehouse Floor Plan)</Title>

            <Row gutter={16} style={{ marginBottom: 16 }}>
                <Col>
                    <Select
                        style={{ width: 160 }}
                        value={floor}
                        options={FLOOR_OPTIONS}
                        onChange={setFloor}
                    />
                </Col>
            </Row>

            <Row gutter={16}>
                {/* ===== 3D VIEWER ===== */}
                <Col span={16}>
                    <Card
                        bodyStyle={{ padding: 0 }}
                        title={`Tầng ${floor} — ${cells.length} vị trí`}
                    >
                        {loading ? (
                            <div style={{ textAlign: "center", padding: 60 }}>
                                <Spin />
                            </div>
                        ) : cells.length === 0 ? (
                            <div style={{ padding: 40 }}>
                                <Empty description="Chưa có vị trí lưu trữ ở tầng này" />
                            </div>
                        ) : (
                            <WarehouseFloorPlan3D
                                cells={cells}
                                selectedCode={selectedCell?.storageLocationNo}
                                onSelectCell={handleSelectCell}
                                height={520}
                            />
                        )}
                    </Card>

                    <div style={{ marginTop: 8 }}>
                        <Tag color="#52c41a">Đang dùng</Tag>
                        <Tag color="#bfbfbf">Trống</Tag>
                        <Tag color="#ff4d4f">Báo phế</Tag>
                        <Text type="secondary" style={{ marginLeft: 8 }}>
                            Chiều cao cột tỉ lệ theo số lượng tài sản. Click vào 1 ô để xem chi tiết.
                        </Text>
                    </div>
                </Col>

                {/* ===== PANEL CHI TIẾT ===== */}
                <Col span={8}>
                    <Card title="Chi tiết vị trí" style={{ minHeight: 520 }}>
                        {!selectedCell ? (
                            <Empty description="Chọn 1 vị trí trong sơ đồ 3D" />
                        ) : (
                            <>
                                <p>
                                    <b>{selectedCell.storageLocationNo}</b> — {selectedCell.nameVN}
                                </p>
                                <p>Bộ phận: {selectedCell.departmentName}</p>
                                <p>Trạng thái: {STATUS_LABEL[selectedCell.status]}</p>
                                <p>Số lượng tài sản: {selectedCell.assetCount}</p>

                                <List
                                    loading={loadingAssets}
                                    dataSource={assets}
                                    size="small"
                                    renderItem={(a) => (
                                        <List.Item
                                            actions={
                                                a.model3DUrl
                                                    ? [
                                                        <a
                                                            key="view3d"
                                                            onClick={() => setPreviewUrl(a.model3DUrl!)}
                                                        >
                                                            Xem 3D
                                                        </a>,
                                                    ]
                                                    : []
                                            }
                                        >
                                            <List.Item.Meta
                                                title={a.assetNo}
                                                description={`${a.assetFullName} — ${a.modelName}`}
                                            />
                                        </List.Item>
                                    )}
                                />
                            </>
                        )}
                    </Card>

                    {previewUrl && (
                        <Card title="Model 3D" style={{ marginTop: 16 }}>
                            <Asset3DViewer modelUrl={previewUrl} height={260} />
                        </Card>
                    )}
                </Col>
            </Row>
        </>
    );
};

export default WarehouseFloorPlanPage;