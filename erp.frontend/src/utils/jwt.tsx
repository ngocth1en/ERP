export interface JwtPayload {
  uid: string;
  system: string;
  systems: string[];   // ✅ thêm
  username:string;
  perm: string;
  exp: number;
}

export const decodeJwt = (token: string): JwtPayload => {
  if (!token) throw new Error("Token missing");

  const parts = token.split(".");
  if (parts.length !== 3) throw new Error("Invalid JWT");

  const raw = JSON.parse(atob(parts[1]));

  return {
    uid: raw.uid,
    system: raw.system,
    perm: raw.perm,
    exp: raw.exp,
    username:raw.username,
    systems: raw.systems ? JSON.parse(raw.systems) : [], // ⭐ QUAN TRỌNG
  };
};

