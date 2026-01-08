import axios from "axios";


/**
 * Đổi mật khẩu
 */
export const changePassword = (
  userName: string,
  data: {
    oldPassword: string;
    newPassword: string;
  }
) => {
  return axios.post(
    `/users/change-password?userName=${userName}`,
    data
  );
};

/**
 * Quên mật khẩu (reset qua email)
 */
export const forgotPasswordByEmail = (data: {
  email: string;
}) => {
  return axios.post(
    `/users/forgot-password/email`,
    data
  );
};

/**
 * (Chuẩn bị cho tương lai)
 * Lấy thông tin user từ token (hiện tại FE dùng auth.store)
 */
// export const getProfile = () => {
//   return axiosClient.get("/users/profile");
// };
