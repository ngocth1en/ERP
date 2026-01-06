import { useFrame } from "@react-three/fiber";
import { Group } from "three";
import { useRef } from "react";
import * as THREE from "three";
import { useMouseParallax } from "../../shared/useMouseParallax";

export default function SnowmanLook() {
  const headRef = useRef<Group>(null);
  const mouse = useMouseParallax();

  useFrame(() => {
    if (!headRef.current) return;

    headRef.current.rotation.y = THREE.MathUtils.lerp(
      headRef.current.rotation.y,
      mouse.x * 0.6,
      0.08
    );

    headRef.current.rotation.x = THREE.MathUtils.lerp(
      headRef.current.rotation.x,
      mouse.y * 0.3,
      0.08
    );
  });

  return (
    <group position={[0, -1, 0]}>
      {/* Body */}
      <mesh>
        <sphereGeometry args={[1, 32, 32]} />
        <meshStandardMaterial color="#ffffff" />
      </mesh>

      {/* Head */}
      <group ref={headRef} position={[0, 1.4, 0]}>
        <mesh>
          <sphereGeometry args={[0.6, 32, 32]} />
          <meshStandardMaterial color="#ffffff" />
        </mesh>

        {/* Eyes */}
        <mesh position={[-0.2, 0.1, 0.55]}>
          <sphereGeometry args={[0.05]} />
          <meshStandardMaterial color="black" />
        </mesh>
        <mesh position={[0.2, 0.1, 0.55]}>
          <sphereGeometry args={[0.05]} />
          <meshStandardMaterial color="black" />
        </mesh>
      </group>
    </group>
  );
}
