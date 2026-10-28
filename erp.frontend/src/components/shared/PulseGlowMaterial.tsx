import { shaderMaterial } from "@react-three/drei";
import { extend, type ThreeElement } from "@react-three/fiber";
import * as THREE from "three";

/**
 * Custom GLSL Shader Material — hiệu ứng "pulse glow" cho ô tài sản được chọn
 * trong sơ đồ kho 3D (CAD/3D Visualization nâng cao).
 *
 * Kỹ thuật: viết vertex/fragment shader thủ công thay vì dùng MeshStandardMaterial
 * có sẵn, điều khiển màu sắc + cường độ phát sáng theo thời gian (uTime) để tạo
 * hiệu ứng nhấp nháy mượt mà (sine wave) khi người dùng click chọn 1 vị trí.
 */
export const PulseGlowMaterial = shaderMaterial(
    // ===== Uniforms =====
    {
        uTime: 0,
        uColor: new THREE.Color("#1677ff"),
        uIntensity: 1.0,
    },
  // ===== Vertex Shader =====
  /* glsl */ `
    varying vec3 vNormal;
    varying vec3 vPosition;

    void main() {
      vNormal = normalize(normalMatrix * normal);
      vPosition = position;
      gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
    }
  `,
  // ===== Fragment Shader =====
  /* glsl */ `
    uniform float uTime;
    uniform vec3 uColor;
    uniform float uIntensity;

    varying vec3 vNormal;
    varying vec3 vPosition;

    void main() {
      // Hiệu ứng nhấp nháy theo sóng sine, dao động trong khoảng [0.4, 1.0]
      float pulse = 0.4 + 0.6 * abs(sin(uTime * 2.5));

      // Fresnel đơn giản: viền sáng hơn phần giữa khi nhìn từ góc nghiêng
      vec3 viewDir = normalize(cameraPosition - vPosition);
      float fresnel = pow(1.0 - max(dot(viewDir, vNormal), 0.0), 2.0);

      vec3 finalColor = uColor * (pulse * uIntensity) + uColor * fresnel * 0.6;

      gl_FragColor = vec4(finalColor, 0.85);
    }
  `
);

extend({ PulseGlowMaterial });

// ===== Khai báo type cho JSX (React Three Fiber) =====
declare module "@react-three/fiber" {
    interface ThreeElements {
        pulseGlowMaterial: ThreeElement<typeof PulseGlowMaterial>;
    }
}