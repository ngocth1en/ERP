import axios from "axios";

export interface LoginRequestDto {
  username: string;
  password: string;
  system: string;
}

export interface LoginResponseDto {
  token: string;
  systems: string[];
  currentSystem: string;
  permission: string; // "1000000000"
}

export const loginApi = async (
  data: LoginRequestDto
): Promise<LoginResponseDto> => {
  const res = await axios.post<LoginResponseDto>(
    "/api/auth/login",
    data
  );
  return res.data;
};
