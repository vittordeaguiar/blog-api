import type { RouteObject } from "react-router-dom";
import LoginPage from "@/features/auth/pages/LoginPage";
import PostsListPage from "@/features/posts/pages/PostsListPage";
import RootLayout from "@/shared/layouts/RootLayout";

export const routes: RouteObject[] = [
  {
    path: "/",
    element: <RootLayout />,
    children: [
      {
        index: true,
        element: <PostsListPage />,
      },
      {
        path: "login",
        element: <LoginPage />,
      },
    ],
  },
  // {
  //   path: "posts/new",
  //   element: (
  //     <ProtectedRoute>
  //       <PostFormPage />
  //     </ProtectedRoute>
  //   ),
  // },
  // {
  //   path: "categories",
  //   element: (
  //     <ProtectedRoute requireRole="Admin">
  //       <CategoriesPage />
  //     </ProtectedRoute>
  //   ),
  // },
];
