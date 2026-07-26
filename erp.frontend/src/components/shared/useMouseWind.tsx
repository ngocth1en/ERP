import { useEffect, useRef } from "react";

export function useMouseWind() {
  const wind = useRef({ x: 0, y: 0 });
  const target = useRef({ x: 0, y: 0 });

  useEffect(() => {
    const onMove = (e: MouseEvent) => {
      target.current.x = (e.clientX / window.innerWidth - 0.5) * 2;
      target.current.y = (e.clientY / window.innerHeight - 0.5) * 2;
    };
    window.addEventListener("mousemove", onMove);
    return () => window.removeEventListener("mousemove", onMove);
  }, []);

  // smoothing
  wind.current.x += (target.current.x - wind.current.x) * 0.05;
  wind.current.y += (target.current.y - wind.current.y) * 0.05;

  return wind.current;
}
