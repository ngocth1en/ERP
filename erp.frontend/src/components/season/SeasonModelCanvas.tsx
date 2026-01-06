import { Canvas } from "@react-three/fiber";
import type { SeasonKey } from "../../utils/season";
import SnowmanLook from "./winter/SnowmanLook";
import "./seasonModel.css";
import AutumnLeaves from "./autumn/AutumnLeaves";
import SummerSun from "./summer/SummerSun";
import GodOfWealthLook from "./spring/GodOfWealthLook";

export const SeasonModelCanvas = ({ season }: { season: SeasonKey }) => {
  return (
    <Canvas
      camera={{ position: [0, 1.2, 4], fov: 45 }}
      gl={{ alpha: true, antialias: true }}
    >
      <ambientLight intensity={0.7} />
      <directionalLight position={[3, 5, 3]} intensity={1.2} />

      {season === "spring" && <GodOfWealthLook />}
      {season === "summer" && <SummerSun />}
      {season === "autumn" && <AutumnLeaves />}
      {season === "winter" && <SnowmanLook />}
    </Canvas>
  );
};
