import { create } from "zustand";
import { decodeJwt } from "../utils/jwt";
import type { SystemCode } from "../constants/system";
import  {  isSystemCode } from "../constants/system";
import { persist } from "zustand/middleware";


interface AuthState {
  isAuthenticated: boolean;
  systemCode?: SystemCode;
  systems: SystemCode[];
  username?:string;
  token?: string;

  login: (token: string) => void;
  switchSystem: (system: SystemCode) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  isAuthenticated: false,
  systems: [],
  systemCode: undefined,
  username:undefined,
  token: undefined,

  login: (token) => {
    const payload = decodeJwt(token);

    // ✅ LỌC SYSTEM HỢP LỆ
    const validSystems = payload.systems.filter(isSystemCode);

    set({
      isAuthenticated: true,
      token,
      systems: validSystems,
      systemCode: validSystems[0],   // system mặc định
      username:payload.username,
    });
  },

  switchSystem: (system) => set({ systemCode: system }),

  logout: () =>
    set({
      isAuthenticated: false,
      systems: [],
      systemCode: undefined,
      username:undefined,
      token: undefined,
    }),
}));
