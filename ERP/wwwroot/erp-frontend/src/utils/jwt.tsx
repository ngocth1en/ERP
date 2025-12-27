export interface JwtPayload {
  sub: string;
  username: string;
  systems: string[];
  permission: string; // "1000000000"
}

export const decodeJwt = (token: string): JwtPayload => {
  const payload = token.split(".")[1];
  return JSON.parse(atob(payload));
};
