import axios from "axios";

export enum ReportPeriodType {
  Month = 1,
  Quarter = 2,
  Year = 3,
}

export interface AssetReportSearchDto {
  periodType: ReportPeriodType;
  year: number;
  month?: number;
  quarter?: number;
  bigTypeId?: number;
  middleTypeId?: number;
  smallTypeId?: number;
  modelId?: number;
  departmentId?: string;
}

export interface AssetReportByDepartmentDto {
  departmentId: string;
  departmentName: string;
  notUsedCount: number;
  waitingCount: number;
  inUseCount: number;
  scrapCount: number;
  liquidatedCount: number;
  totalCount: number;
}

export interface AssetReportResultDto {
  periodLabel: string;
  items: AssetReportByDepartmentDto[];
  grandTotal: number;
}

export const searchAssetReport = (dto: AssetReportSearchDto) =>
  axios.post<AssetReportResultDto>("/api/asset-reports/search", dto);

export const exportAssetReport = async (dto: AssetReportSearchDto) => {
  const res = await axios.post("/api/asset-reports/export", dto, {
    responseType: "blob",
  });
  const url = window.URL.createObjectURL(new Blob([res.data]));
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", `BaoCaoTaiSan_${Date.now()}.xlsx`);
  document.body.appendChild(link);
  link.click();
  link.remove();
};