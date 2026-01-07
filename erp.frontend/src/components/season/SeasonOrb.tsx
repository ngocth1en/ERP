import { useFrame } from "@react-three/fiber";
import { MeshTransmissionMaterial } from "@react-three/drei";
import { useRef } from "react";
import * as THREE from "three";
import type { SeasonKey } from "../../utils/season";

type Props = {
  season: SeasonKey;
};

const seasonConfig: Record<
  SeasonKey,
  {
    color: string;
    emissive: string;
    roughness: number;
    transmission: number;
  }
> = {
  spring: {
    color: "#9fffc7",
    emissive: "#6be8a3",
    roughness: 0.3,
    transmission: 0.6
  },
  summer: {
    color: "#89f7fe",
    emissive: "#ffd86f",
    roughness: 0.1,
    transmission: 0.9
  },
  autumn: {
    color: "#ffb347",
    emissive: "#ff7a18",
    roughness: 0.5,
    transmission: 0.4
  },
  winter: {
    color: "#dbeafe",
    emissive: "#93c5fd",
    roughness: 0.05,
    transmission: 1
  }
};

export const SeasonOrb = ({ season }: Props) => {
  const ref = useRef<THREE.Mesh>(null!);
  const cfg = seasonConfig[season];

  useFrame((_, delta) => {
    ref.current.rotation.y += delta * 0.25;
    ref.current.rotation.x += delta * 0.1;
  });

  return (
    <mesh ref={ref}>
      <sphereGeometry args={[1.2, 64, 64]} />
      <MeshTransmissionMaterial
        color={cfg.color}
        emissive={cfg.emissive}
        roughness={cfg.roughness}
        transmission={cfg.transmission}
        thickness={0.8}
        chromaticAberration={0.03}
        anisotropicBlur={0.1}
        distortion={0.15}
        distortionScale={0.3}
        temporalDistortion={0.15}
      />
    </mesh>
  );
};
