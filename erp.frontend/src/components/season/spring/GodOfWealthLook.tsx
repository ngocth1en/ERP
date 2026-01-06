import { useFrame } from "@react-three/fiber";
import { Group } from "three";
import { useRef } from "react";
import * as THREE from "three";
import { useMouseParallax } from "../../shared/useMouseParallax";

export default function GodOfWealthLook() {
  const headRef = useRef<Group>(null);
  const mouse = useMouseParallax();

  useFrame(() => {
    if (!headRef.current) return;

    headRef.current.rotation.y = THREE.MathUtils.lerp(
      headRef.current.rotation.y,
      mouse.x * 0.5,
      0.08
    );

    headRef.current.rotation.x = THREE.MathUtils.lerp(
      headRef.current.rotation.x,
      mouse.y * 0.25,
      0.08
    );
  });

  return (
    <group position={[0, -1.2, 0]} scale={1.05}>
      {/* ================= BODY (áo đỏ) ================= */}
      <mesh>
        <cylinderGeometry args={[1.1, 1.25, 1.6, 48]} />
        <meshStandardMaterial
          color="#c4161c"
          roughness={0.35}
          metalness={0.05}
        />
      </mesh>

      {/* Viền áo vàng */}
      <mesh position={[0, -0.75, 0]}>
        <cylinderGeometry args={[1.18, 1.32, 0.15, 48]} />
        <meshStandardMaterial color="#f2c14e" />
      </mesh>

      {/* ================= HEAD ================= */}
      <group ref={headRef} position={[0, 1.1, 0]}>
        {/* Face */}
        <mesh>
          <sphereGeometry args={[0.62, 48, 48]} />
          <meshStandardMaterial
            color="#ffd9b3"
            roughness={0.4}
          />
        </mesh>

        {/* Cheeks */}
        <mesh position={[-0.28, -0.05, 0.52]}>
          <sphereGeometry args={[0.12, 32, 32]} />
          <meshStandardMaterial color="#ffb3b3" />
        </mesh>
        <mesh position={[0.28, -0.05, 0.52]}>
          <sphereGeometry args={[0.12, 32, 32]} />
          <meshStandardMaterial color="#ffb3b3" />
        </mesh>

        {/* Eyes */}
        <mesh position={[-0.18, 0.1, 0.55]}>
          <sphereGeometry args={[0.06, 32, 32]} />
          <meshStandardMaterial color="#111" />
        </mesh>
        <mesh position={[0.18, 0.1, 0.55]}>
          <sphereGeometry args={[0.06, 32, 32]} />
          <meshStandardMaterial color="#111" />
        </mesh>

        {/* ================= BEARD ================= */}
        <mesh position={[0, -0.28, 0.45]}>
          <sphereGeometry args={[0.42, 48, 48]} />
          <meshStandardMaterial color="#ffffff" roughness={0.45} />
        </mesh>

        {/* ================= HAT ================= */}
        {/* Mũ vàng */}
        <mesh position={[0, 0.75, 0]}>
          <cylinderGeometry args={[0.55, 0.6, 0.35, 48]} />
          <meshStandardMaterial color="#f2c14e" />
        </mesh>

        {/* Viền đỏ */}
        <mesh position={[0, 0.58, 0]}>
          <cylinderGeometry args={[0.62, 0.62, 0.12, 48]} />
          <meshStandardMaterial color="#c4161c" />
        </mesh>

        {/* Nút trên mũ */}
        <mesh position={[0, 0.95, 0]}>
          <sphereGeometry args={[0.12, 32, 32]} />
          <meshStandardMaterial color="#f2c14e" />
        </mesh>
      </group>

      {/* ================= GOLD INGOT ================= */}
      <group position={[0.65, -0.15, 0.6]}>
        <mesh>
          <torusGeometry args={[0.22, 0.12, 24, 48]} />
          <meshStandardMaterial
            color="#f5c542"
            roughness={0.25}
            metalness={0.6}
          />
        </mesh>
      </group>
    </group>
  );
}
