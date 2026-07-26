import { useFrame } from "@react-three/fiber";
import { useRef } from "react";
import { useMouseParallax } from "../../shared/useMouseParallax";
import { useGLTF } from "@react-three/drei";
import { Group } from "three";
import * as THREE from "three";

export default function GodOfWealthLook() {
  const headRef = useRef<Group>(null);
  const mouse = useMouseParallax();

  // Load model Spring.glb
  const { scene } = useGLTF("/models/Spring.glb"); 

  useFrame(() => {
    if (!headRef.current) return;

    // rotation Y: xoay trái phải → đúng
    headRef.current.rotation.y = THREE.MathUtils.lerp(
      headRef.current.rotation.y,
      mouse.x * 0.5,  // giữ nguyên
      0.08
    );

    // rotation X: xoay lên xuống → đảo dấu
    headRef.current.rotation.x = THREE.MathUtils.lerp(
      headRef.current.rotation.x,
      -mouse.y * 0.25,  // thêm dấu âm để đảo chiều
      0.08
    );
  });


  return (
    <group position={[0, 0.5, 0]} scale={1.05}>
      {/* Clone scene từ GLTF */}
      <primitive ref={headRef} object={scene} />
    </group>
  );
}
