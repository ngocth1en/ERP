import axios from "axios";

export interface LoginRequestDto {
  username: string;
  password: string;
  systemCode: string;
}

export interface LoginResponseDto {
  accessToken: string;
  systems: string[];
  currentSystem: string;
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
