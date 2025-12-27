import { createBrowserRouter } from "react-router-dom";
import Login from "../auth/Login";
import MainLayout from "../layouts/MainLayout";
import ProtectedRoute from "./ProtectedRoutes";

const router = createBrowserRouter([
  {
    path: "/login",
    element: <Login />,
  },

  {
    path: "/asm",
    element: (
      <ProtectedRoute system="ASM">
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <div>ASM Dashboard</div> },
      { path: "assets", element: <div>Quản lý tài sản</div> },
      { path: "categories", element: <div>Danh mục</div> },
    ],
  },

  {
    path: "/qcm",
    element: (
      <ProtectedRoute system="QCM">
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <div>QCM Dashboard</div> },
      { path: "qc", element: <div>Kiểm tra chất lượng</div> },
    ],
  },

  {
    path: "/hrm",
    element: (
      <ProtectedRoute system="HRM">
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <div>HRM Dashboard</div> },
      { path: "employees", element: <div>Nhân sự</div> },
    ],
  },
]);

export default router;
