import { useFrame } from "@react-three/fiber";
import { useRef } from "react";
import * as THREE from "three";
import { useMouseWind } from "../../shared/useMouseWind";
import { useMouseParallax } from "../../shared/useMouseParallax";

export default function SummerSun() {
  const group = useRef<THREE.Group>(null!);
  const heatRings = useRef<THREE.Mesh[]>([]);
  const sunCore = useRef<THREE.Mesh>(null!);

  const wind = useMouseParallax();

  useFrame((state) => {
    const t = state.clock.elapsedTime;

    /* 🌬️ HƯỚNG NHIỆT / GIÓ */
    if (group.current) {
      group.current.rotation.z = THREE.MathUtils.lerp(
        group.current.rotation.z,
        wind.x * 0.3,
        0.06
      );

      group.current.rotation.x = THREE.MathUtils.lerp(
        group.current.rotation.x,
        wind.y * 0.15,
        0.06
      );
    }

    /* ☀️ SUN BREATHING */
    if (sunCore.current) {
      const s = 1 + Math.sin(t * 1.5) * 0.015;
      sunCore.current.scale.set(s, s, s);
    }

    /* 🔥 HEAT WAVES */
    heatRings.current.forEach((ring, i) => {
      ring.rotation.z += 0.0015 + i * 0.001;
      const wave = 1 + Math.sin(t * 1.2 + i * 2) * 0.025;
      ring.scale.set(wave, wave, 1);
    });
  });

  return (
    <group ref={group}>
      {/* ☀️ SUN CORE */}
      <mesh ref={sunCore}>
        <sphereGeometry args={[0.75, 32, 32]} />
        <meshStandardMaterial
          color="#ffb300"
          emissive="#ffdd55"
          emissiveIntensity={1}
        />
      </mesh>

      {/* 🌕 HALO */}
      <mesh>
        <sphereGeometry args={[1.1, 32, 32]} />
        <meshStandardMaterial
          transparent
          opacity={0.18}
          emissive="#ffe082"
        />
      </mesh>

      {/* 🔥 HEAT RINGS */}
      {Array.from({ length: 3 }).map((_, i) => (
        <mesh
          key={i}
          ref={(el) => el && (heatRings.current[i] = el)}
          position={[0, 0, -0.12 * i]}
        >
          <torusGeometry args={[1.25 + i * 0.15, 0.035, 8, 64]} />
          <meshStandardMaterial
            color="#fff3c0"
            transparent
            opacity={0.25}
          />
        </mesh>
      ))}

      {/* 🌊 WATER HEAT RIPPLE */}
      <mesh position={[0, -1.35, 0]}>
        <ringGeometry args={[0.85, 1.35, 64]} />
        <meshStandardMaterial
          color="#4fc3f7"
          transparent
          opacity={0.45}
        />
      </mesh>
    </group>
  );
}
