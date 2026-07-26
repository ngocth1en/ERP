import { useEffect, useState } from "react";
import type { SeasonKey } from "../../utils/season";
import "./season.css";

interface Particle {
  id: number;
  left: number;
  size: number;
  duration: number;
  delay: number;
  drift: number;
}

const spawnParticles = (count: number): Particle[] =>
  Array.from({ length: count }).map((_, i) => ({
    id: i,
    left: Math.random() * 100,
    size: 12 + Math.random() * 14,
    duration: 6 + Math.random() * 10,
    delay: Math.random() * -10,
    drift: Math.random() * 80 - 40
  }));

export const SeasonParticles = ({ season }: { season: SeasonKey }) => {
  const [particles, setParticles] = useState<Particle[]>([]);

  useEffect(() => {
    if (season === "summer") return;
    setParticles(spawnParticles(18));
  }, [season]);

  if (season === "summer") return <div className="sun-core" />;

  const icon =
    season === "winter"
      ? "❄️"
      : season === "spring"
      ? "🌸"
      : "🍁";

  return (
    <>
      {particles.map((p) => (
        <span
          key={p.id}
          className={`particle particle-${season}`}
          style={{
            left: `${p.left}%`,
            fontSize: p.size,
            animationDuration: `${p.duration}s`,
            animationDelay: `${p.delay}s`,
            ["--drift" as any]: `${p.drift}px`
          }}
        >
          {icon}
        </span>
      ))}
    </>
  );
};
