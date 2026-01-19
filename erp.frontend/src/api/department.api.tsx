import axios from "axios";
export interface DepartmentOptionDto {
  id: string;
  name: string;
}

export const getDepartmentOptions = () => {
  return axios.get<DepartmentOptionDto[]>(
    "/api/departments/options"
  );
};
