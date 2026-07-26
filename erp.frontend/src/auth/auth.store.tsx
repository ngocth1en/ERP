import { create } from "zustand";
import { persist, createJSONStorage } from "zustand/middleware";
import { decodeJwt } from "../utils/jwt";
import type { SystemCode } from "../constants/system";
import { isSystemCode } from "../constants/system";

interface AuthState {
  isAuthenticated: boolean;

  systemCode?: SystemCode;
  systems: SystemCode[];
  username?: string;
  token?: string;
  
  currentMenuCode?: string;
  actionPermission: string;

  login: (token: string) => void;
  switchSystem: (system: SystemCode) => void;
  setCurrentMenu: (menuCode: string, permission: string) => void;
  clearMenuPermission: () => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      isAuthenticated: false,
      systems: [],
      systemCode: undefined,
      username: undefined,
      token: undefined,

      currentMenuCode: undefined,
      actionPermission: "0000000000",

      login: (token) => {
        const payload = decodeJwt(token);
        const validSystems = payload.systems.filter(isSystemCode);

        set({
          isAuthenticated: true,
          token,
          systems: validSystems,
          systemCode: validSystems[0],
          username: payload.username,
          currentMenuCode: undefined,
          actionPermission: "0000000000",
        });
      },

      switchSystem: (system) =>
        set({
          systemCode: system,
          currentMenuCode: undefined,
          actionPermission: "0000000000",
        }),

      setCurrentMenu: (menuCode, permission) =>
        set({
          currentMenuCode: menuCode,
          actionPermission: permission ?? "0000000000",
        }),

      clearMenuPermission: () =>
        set({
          currentMenuCode: undefined,
          actionPermission: "0000000000",
        }),

      logout: () =>
        set({
          isAuthenticated: false,
          systems: [],
          systemCode: undefined,
          username: undefined,
          token: undefined,
          currentMenuCode: undefined,
          actionPermission: "0000000000",
        }),
    }),
      {
          name: "auth-store",
          storage: createJSONStorage(() => sessionStorage), // ✅ tắt browser = hết phiên
          partialize: (s) => ({
              isAuthenticated: s.isAuthenticated,
              token: s.token,
              systems: s.systems,
              systemCode: s.systemCode,
              username: s.username,
              currentMenuCode: s.currentMenuCode,
              actionPermission: s.actionPermission,
          }),
    }
  )
);
