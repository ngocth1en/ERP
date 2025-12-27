// sidebar.menu.ts
import type { ReactNode } from "react";

export interface SidebarMenuItem {
  key: string;
  icon: ReactNode;
  label: string;
  path?: string;
  disabled?: boolean;
}
