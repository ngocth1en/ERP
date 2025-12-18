import { createBrowserRouter } from "react-router-dom";
import MainLayout from "../layouts/MainLayout";
import Login from "../auth/Login";
import ProtectedRoute from "../routes/ProtectedRoutes";

const router = createBrowserRouter([
    {
      path: "/login",
      element: <Login />,
    },
    {
      path: "/",
      element: (
        <ProtectedRoute>
          <MainLayout />
        </ProtectedRoute>
      ),
      children: [
        {
          index: true,
          element: <div>Dashboard</div>,
        },
        {
          path: "assets",
          element: <div>Asset Management</div>,
        },
      ],
    },
  ]);

export default router;
