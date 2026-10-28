import { useMemo, useRef } from "react";
import { Canvas, useFrame, type ThreeEvent } from "@react-three/fiber";
import { OrbitControls, Text, Grid } from "@react-three/drei";
import * as THREE from "three";
import type { WarehouseCellDto } from "../../api/location.api";
import { PulseGlowMaterial } from "./PulseGlowMaterial";

const CELL_SIZE = 1.6;
const CELL_GAP = 0.3;

/* ================= MÀU THEO TRẠNG THÁI ================= */
// 1 = Đang dùng (xanh), 2 = Trống (xám), 3 = Báo phế (đỏ)
function getColorByStatus(status: number): string {
    return status === 1 ? "#52c41a" : status === 3 ? "#ff4d4f" : "#bfbfbf";
}

/* ================= HIỆU ỨNG PULSE GLOW (custom GLSL shader) ================= */
interface PulseGlowBoxProps {
    size: [number, number, number];
}

/**
 * Lớp vỏ bọc ngoài ô được chọn, dùng custom shader (PulseGlowMaterial)
 * để tạo hiệu ứng nhấp nháy + fresnel glow — minh họa kỹ thuật WebGL/CAD nâng cao
 * (viết GLSL thủ công) thay vì chỉ dùng vật liệu có sẵn của Three.js.
 */
function PulseGlowBox({ size }: PulseGlowBoxProps) {
    const materialRef = useRef<any>(null);

    useFrame(({ clock }) => {
        if (materialRef.current) {
            materialRef.current.uTime = clock.getElapsedTime();
        }
    });

    return (
        <mesh scale={[1.08, 1.04, 1.08]}>
            <boxGeometry args={size} />
            {/* @ts-ignore - custom shader material khai báo qua extend() */}
            <pulseGlowMaterial
                ref={materialRef}
                uColor={new THREE.Color("#1677ff")}
                uIntensity={1.1}
                transparent
                depthWrite={false}
                side={THREE.DoubleSide}
            />
        </mesh>
    );
}

/* ================= 1 Ô VỊ TRÍ ================= */
interface CellBoxProps {
    cell: WarehouseCellDto;
    selected: boolean;
    onSelect: (cell: WarehouseCellDto) => void;
}

function CellBox({ cell, selected, onSelect }: CellBoxProps) {
    const meshRef = useRef<THREE.Mesh>(null);

    const x = cell.gridX * (CELL_SIZE + CELL_GAP);
    const z = cell.gridZ * (CELL_SIZE + CELL_GAP);

    // Chiều cao cột tỉ lệ theo số lượng tài sản (data-driven visualization)
    const height = 0.3 + Math.min(cell.assetCount, 20) * 0.1;
    const boxSize: [number, number, number] = [CELL_SIZE, height, CELL_SIZE];

    const handleClick = (e: ThreeEvent<MouseEvent>) => {
        e.stopPropagation();
        onSelect(cell);
    };

    return (
        <group position={[x, height / 2, z]}>
            <mesh
                ref={meshRef}
                onClick={handleClick}
                onPointerOver={(e) => {
                    e.stopPropagation();
                    document.body.style.cursor = "pointer";
                }}
                onPointerOut={() => {
                    document.body.style.cursor = "default";
                }}
            >
                <boxGeometry args={boxSize} />
                <meshStandardMaterial color={getColorByStatus(cell.status)} />
            </mesh>

            {/* ===== Hiệu ứng pulse glow khi được chọn (custom GLSL shader) ===== */}
            {selected && <PulseGlowBox size={boxSize} />}

            <Text
                position={[0, height / 2 + 0.3, 0]}
                fontSize={0.18}
                color="#333"
                anchorX="center"
                anchorY="middle"
            >
                {cell.storageLocationNo}
            </Text>
        </group>
    );
}

/* ================= SCENE ================= */
interface SceneProps {
    cells: WarehouseCellDto[];
    selectedCode?: string;
    onSelect: (cell: WarehouseCellDto) => void;
}

function Scene({ cells, selectedCode, onSelect }: SceneProps) {
    const gridSize = useMemo(() => {
        const maxX = Math.max(1, ...cells.map((c) => c.gridX));
        const maxZ = Math.max(1, ...cells.map((c) => c.gridZ));
        return Math.max(maxX, maxZ) * (CELL_SIZE + CELL_GAP) + 6;
    }, [cells]);

    return (
        <>
            <ambientLight intensity={0.8} />
            <directionalLight position={[8, 10, 6]} intensity={1.1} castShadow />

            <Grid
                args={[gridSize, gridSize]}
                cellSize={CELL_SIZE + CELL_GAP}
                cellColor="#d9d9d9"
                sectionColor="#bfbfbf"
                fadeDistance={gridSize * 1.5}
                position={[0, 0, 0]}
            />

            {cells.map((cell) => (
                <CellBox
                    key={cell.storageLocationNo}
                    cell={cell}
                    selected={cell.storageLocationNo === selectedCode}
                    onSelect={onSelect}
                />
            ))}
        </>
    );
}

/* ================= VIEWER CHÍNH ================= */
interface WarehouseFloorPlan3DProps {
    cells: WarehouseCellDto[];
    selectedCode?: string;
    onSelectCell: (cell: WarehouseCellDto) => void;
    height?: number | string;
}

/**
 * Sơ đồ kho 3D theo tầng: mỗi vị trí lưu trữ là 1 cột (box) với
 * - Màu sắc theo trạng thái (Đang dùng/Trống/Báo phế)
 * - Chiều cao tỉ lệ theo số lượng tài sản
 * - Raycasting click để xem chi tiết tài sản tại vị trí đó
 * - Custom GLSL shader (pulse glow) highlight ô được chọn
 * - OrbitControls để xoay/zoom/pan quan sát toàn kho
 */
export const WarehouseFloorPlan3D = ({
    cells,
    selectedCode,
    onSelectCell,
    height = 500,
}: WarehouseFloorPlan3DProps) => {
    return (
        <div style={{ width: "100%", height }}>
            <Canvas
                shadows
                camera={{ position: [6, 8, 10], fov: 50 }}
                gl={{ antialias: true }}
            >
                <Scene cells={cells} selectedCode={selectedCode} onSelect={onSelectCell} />
                <OrbitControls makeDefault />
            </Canvas>
        </div>
    );
};

export default WarehouseFloorPlan3D;