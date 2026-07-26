import { createBrowserRouter } from "react-router-dom";
import Login from "../auth/Login";
import MainLayout from "../layouts/MainLayout";
import ProtectedRoute from "./ProtectedRoutes";
import GuestRoute from "./GuestRoute";

const router = createBrowserRouter([
  {
    path: "/login",
    element: (
      <GuestRoute>
        <Login />
      </GuestRoute>
    ),
  },

  {
    path: "/asset",
    element: (
      <ProtectedRoute system="ASSET">
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
    path: "/qc",
    element: (
      <ProtectedRoute system="QC">
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <div>QCM Dashboard</div> },
      { path: "qc", element: <div>Kiểm tra chất lượng</div> },
    ],
  },

  {
    path: "/maint",
    element: (
      <ProtectedRoute system="MAINT">
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <div>Dashboard</div> },
      { path: "employees", element: <div>Hệ thống</div> },
    ],
  },
  {
    path: "/develop",
    element: (
      <ProtectedRoute system="DEVELOP">
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <div>Dashboard</div> },
      { path: "temp", element: <div>Hệ thống</div> },
    ],
  },
]);

export default router;
