export type SeasonKey = "spring" | "summer" | "autumn" | "winter";

export const getSeasonKey = (): SeasonKey => {
  const m = new Date().getMonth() + 1;
  if (m <= 3) return "spring";
  if (m <= 6) return "summer";
  if (m <= 9) return "autumn";
  return "winter";
};
