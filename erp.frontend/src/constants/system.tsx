export const SYSTEM_CODES = ["ASSET", "MAINT", "QC"] as const;

export type SystemCode = typeof SYSTEM_CODES[number];

export const isSystemCode = (value: string): value is SystemCode =>
  SYSTEM_CODES.includes(value as SystemCode);
