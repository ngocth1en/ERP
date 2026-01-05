// system.api.ts
import axios from "axios";

export interface SystemOptionDto {
  code: string;
  name: string;
}

export const getSystems = async (): Promise<SystemOptionDto[]> => {
  const res = await axios.get<SystemOptionDto[]>("/api/systems");
  return res.data;
};
// export const getSystemsByUser = async (
//   username: string
// ): Promise<SystemOptionDto[]> => {
//   if (!username) return [];
//   const res = await axios.get<SystemOptionDto[]>(
//     `/api/systems/by-user/${username}`
//   );
//   return res.data;
// };
export const getSystemsByUser = async (username: string) => {
  const res = await axios.get<SystemOptionDto[]>(
    `/api/systems/by-user/${username}` 
  );
  return res.data; 
};
