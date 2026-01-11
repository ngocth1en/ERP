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
    `/api/users/change-password?userName=${userName}`,
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
    `/api/users/forgot-password/email`,
    data
  );
};
export const getUsers = () => {
  return axios.get("/api/users").then(res => res.data);
};

