import { ActionIndex } from "../constants/action";

export const hasPermission = (
  permission: string,
  action: ActionIndex
): boolean => {
  return permission[action] === "1";
};
