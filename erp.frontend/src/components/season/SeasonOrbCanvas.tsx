import { Canvas } from "@react-three/fiber";
import { SeasonOrb } from "./SeasonOrb";
import type { SeasonKey } from "../../utils/season";

export const SeasonOrbCanvas = ({ season }: { season: SeasonKey }) => {
  return (
    <Canvas
      camera={{ position: [0, 0, 4], fov: 50 }}
      gl={{ antialias: true, alpha: true }}
    >
      <ambientLight intensity={0.6} />
      <directionalLight position={[3, 3, 3]} intensity={1.2} />
      <SeasonOrb season={season} />
    </Canvas>
  );
};
