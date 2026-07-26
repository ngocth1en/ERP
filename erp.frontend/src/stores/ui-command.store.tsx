import { create } from "zustand";

export type UiCommand =
  | "QUERY"
  | "INSERT"
  | "MODIFY"
  | "DELETE"
  | "CONFIRM"
  | "UNLOCK"
  | "PRINT"
  | "EXCEL";

interface UiCommandState {
  command: UiCommand | null;
  trigger: (cmd: UiCommand) => void;
  clear: () => void;
}

export const useUiCommandStore = create<UiCommandState>((set) => ({
  command: null,
  trigger: (cmd) => set({ command: cmd }),
  clear: () => set({ command: null }),
}));

