// system.api.ts
import axios from "axios";

export interface SystemOptionDto {
  Code: string;
  Name: string;
}

export const getSystems = async (): Promise<SystemOptionDto[]> => {
  const res = await axios.get<SystemOptionDto[]>("/api/system");
  return res.data;
};
export const getSystemsByUser = async (
  username: string
): Promise<SystemOptionDto[]> => {
  if (!username) return [];
  const res = await axios.get<SystemOptionDto[]>(
    `/api/system/by-user/${username}`
  );
  return res.data;
};