import { useFrame } from "@react-three/fiber";
import { useRef } from "react";
import * as THREE from "three";
import { useMouseWind } from "../../shared/useMouseWind";
import { useMouseParallax } from "../../shared/useMouseParallax";

export default function AutumnLeaves() {
  const leaves = useRef<THREE.Mesh[]>([]);
  const wind = useMouseParallax();

  useFrame(({ clock }) => {
    leaves.current.forEach((leaf, i) => {
      leaf.position.y -= 0.01 + wind.y * 0.02;
      leaf.position.x += wind.x * 0.02;
      leaf.rotation.z += 0.01;

      if (leaf.position.y < -2) {
        leaf.position.y = 2;
      }
    });
  });

  return (
    <>
      {Array.from({ length: 12 }).map((_, i) => (
        <mesh
          key={i}
          ref={(el) => el && (leaves.current[i] = el)}
          position={[
            Math.random() * 2 - 1,
            Math.random() * 2,
            0
          ]}
        >
          <planeGeometry args={[0.25, 0.15]} />
          <meshStandardMaterial
            color="#d2691e"
            side={THREE.DoubleSide}
          />
        </mesh>
      ))}
    </>
  );
}
