import axios from "axios";

/* SEARCH */
export const searchAssetBigTypes = (params: any) =>
  axios.get("/api/asset-big-types/search", { params });

/* CREATE */
export const createAssetBigType = (data: {
  nameEN: string;
  nameVN: string;
}) =>
  axios.post("/api/asset-big-types", {
    nameEN: data.nameEN,
    nameVN: data.nameVN,
  });

/* UPDATE */
export const updateAssetBigType = (
  id: any,
  data: {
    nameEN: string;
    nameVN: string;
  }
) =>
  axios.put(`/api/asset-big-types/${id}`, {
    nameEN: data.nameEN,
    nameVN: data.nameVN,
  });

/* DELETE */
export const deleteAssetBigType = (id: any) =>
  axios.delete(`/api/asset-big-types/${id}`);
