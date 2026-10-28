import { Suspense } from "react";
import { Canvas } from "@react-three/fiber";
import { OrbitControls, useGLTF, Center, Bounds } from "@react-three/drei";

/* ================= MODEL MESH ================= */

interface GlbModelProps {
    url: string;
    onClick?: () => void;
    highlighted?: boolean;
}

function GlbModel({ url, onClick, highlighted }: GlbModelProps) {
    const { scene } = useGLTF(url);

    return (
        <primitive
            object={scene}
            onClick={(e: any) => {
                e.stopPropagation();
                onClick?.();
            }}
            // Highlight đơn giản bằng cách scale nhẹ khi được chọn (raycasting selection)
            scale={highlighted ? 1.05 : 1}
        />
    );
}

/* ================= VIEWER ================= */

interface Asset3DViewerProps {
    modelUrl: string;
    height?: number | string;
    highlighted?: boolean;
    onSelect?: () => void;
    autoRotate?: boolean;
}

/**
 * Component dùng chung để hiển thị model 3D (.glb/.gltf) của tài sản/thiết bị.
 * Hỗ trợ: xoay/zoom (OrbitControls), raycasting click-select (onSelect),
 * tự động canh giữa + fit khung hình (Bounds + Center).
 */
export const Asset3DViewer = ({
    modelUrl,
    height = 300,
    highlighted = false,
    onSelect,
    autoRotate = true,
}: Asset3DViewerProps) => {
    return (
        <div style={{ width: "100%", height }}>
            <Canvas
                camera={{ position: [2, 2, 3], fov: 45 }}
                gl={{ antialias: true, alpha: true }}
            >
                <ambientLight intensity={0.8} />
                <directionalLight position={[3, 5, 3]} intensity={1.2} />

                <Suspense fallback={null}>
                    <Bounds fit clip observe margin={1.2}>
                        <Center>
                            <GlbModel
                                url={modelUrl}
                                onClick={onSelect}
                                highlighted={highlighted}
                            />
                        </Center>
                    </Bounds>
                </Suspense>

                <OrbitControls
                    autoRotate={autoRotate}
                    autoRotateSpeed={1.5}
                    enablePan={false}
                />
            </Canvas>
        </div>
    );
};

export default Asset3DViewer;