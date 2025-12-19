import { lazy, Suspense } from "react";
import type { RouteObject } from "react-router-dom";
import RootLayout from "@/shared/layouts/RootLayout";
import ProtectedRoute from "./ProtectedRoute";

const PostsListPage = lazy(() => import("@/features/posts/pages/PostsListPage"));
const PostDetailPage = lazy(() => import("@/features/posts/pages/PostDetailPage"));
const CreatePostPage = lazy(() => import("@/features/posts/pages/CreatePostPage"));
const LoginPage = lazy(() => import("@/features/auth/pages/LoginPage"));

function PageLoader() {
  return (
    <div className="flex items-center justify-center min-h-[60vh]">
      <div className="flex flex-col items-center gap-4">
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-brand border-t-transparent" />
        <p className="text-sm text-muted-foreground">Carregando...</p>
      </div>
    </div>
  );
}

export const routes: RouteObject[] = [
  {
    path: "/",
    element: <RootLayout />,
    children: [
      {
        index: true,
        element: (
          <Suspense fallback={<PageLoader />}>
            <PostsListPage />
          </Suspense>
        ),
      },
      {
        path: "posts/:slug",
        element: (
          <Suspense fallback={<PageLoader />}>
            <PostDetailPage />
          </Suspense>
        ),
      },
      {
        path: "login",
        element: (
          <Suspense fallback={<PageLoader />}>
            <LoginPage />
          </Suspense>
        ),
      },
      {
        path: "posts/new",
        element: (
          <Suspense fallback={<PageLoader />}>
            <ProtectedRoute>
              <CreatePostPage />
            </ProtectedRoute>
          </Suspense>
        ),
      },
    ],
  },
];
