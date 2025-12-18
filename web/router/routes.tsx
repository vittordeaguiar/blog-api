import type { RouteObject } from "react-router-dom";
import LoginPage from "@/features/auth/pages/LoginPage";
import PostsListPage from "@/features/posts/pages/PostsListPage";
import PostDetailPage from "@/features/posts/pages/PostDetailPage";
import RootLayout from "@/shared/layouts/RootLayout";
import CreatePostPage from "@/features/posts/pages/CreatePostPage";
import ProtectedRoute from "./ProtectedRoute";

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
        path: "posts/:slug",
        element: <PostDetailPage />,
      },
      {
        path: "login",
        element: <LoginPage />,
      },
      {
        path: "posts/new",
        element: (
          <ProtectedRoute>
            <CreatePostPage />
          </ProtectedRoute>
        ),
      },
    ],
  },
];
