import { useFrame } from "@react-three/fiber";
import { useEffect, useRef } from "react";
import { useMouseParallax } from "../../shared/useMouseParallax";
import { useGLTF } from "@react-three/drei";
import * as THREE from "three";

export default function Autumn() {
  const headBone = useRef<THREE.Bone | null>(null);
  const rightArmBone = useRef<THREE.Bone | null>(null);

  const headBase = useRef<THREE.Euler | null>(null);
  const armBase = useRef<THREE.Euler | null>(null);

  const mouse = useMouseParallax();
  const { scene } = useGLTF("/models/Autumn2.glb");

  /* ---------------- Find bones & store default pose ---------------- */
  useEffect(() => {
    scene.traverse((obj) => {
      if (obj instanceof THREE.Bone) {
        if (obj.name === "Head") {
          headBone.current = obj;
          headBase.current = obj.rotation.clone();

          // Default đầu nghiêng nhẹ sang phải
          headBase.current.y += 0.15;
        }

        if (obj.name === "RightArm") {
          rightArmBone.current = obj;
          armBase.current = obj.rotation.clone();
        }
      }
    });

    if (!headBone.current) console.warn("❌ Head bone not found");
    if (!rightArmBone.current) console.warn("❌ RightArm bone not found");
  }, [scene]);

  /* ---------------- Animation loop ---------------- */
  useFrame(() => {
    if (!headBone.current || !headBase.current) return;

    const lerp = 0.12;

    /* ===== HEAD (xoay nhiều hơn) ===== */
    const headX =
      headBase.current.x +
      THREE.MathUtils.clamp(-mouse.y * 0.45, -0.45, 0.4);

    const headY =
      headBase.current.y +
      THREE.MathUtils.clamp(mouse.x * 0.75, -0.75, 0.75);

    headBone.current.rotation.x = THREE.MathUtils.lerp(
      headBone.current.rotation.x,
      headX,
      lerp
    );

    headBone.current.rotation.y = THREE.MathUtils.lerp(
      headBone.current.rotation.y,
      headY,
      lerp
    );

    /* ===== RIGHT ARM (follow mạnh hơn nhưng nhẹ hơn đầu) ===== */
    if (rightArmBone.current && armBase.current) {
      const armX =
        armBase.current.x +
        THREE.MathUtils.clamp(-mouse.y * 0.25, -0.35, 0.35);

      const armY =
        armBase.current.y +
        THREE.MathUtils.clamp(mouse.x * 0.4, -0.5, 0.5);

      rightArmBone.current.rotation.x = THREE.MathUtils.lerp(
        rightArmBone.current.rotation.x,
        armX,
        lerp
      );

      rightArmBone.current.rotation.y = THREE.MathUtils.lerp(
        rightArmBone.current.rotation.y,
        armY,
        lerp
      );
    }
  });

  return (
    <group position={[0, -1, 0]} scale={1.5}>
      <primitive object={scene} />
    </group>
  );
}
