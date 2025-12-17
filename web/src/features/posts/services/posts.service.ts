import type { AxiosResponse } from "axios";
import { api } from "@/shared/lib/axios";
import type { Post, PagedResult, PostsQueryParams } from "../types/post.types";

export const postsService = {
  getPosts: (params?: PostsQueryParams): Promise<AxiosResponse<PagedResult<Post>>> =>
    api.get("/api/v1/posts", { params }),

  getPost: (slug: string): Promise<AxiosResponse<Post>> => api.get(`/api/v1/posts/${slug}`),

  createPost: (data: Partial<Post>): Promise<AxiosResponse<Post>> =>
    api.post("/api/v1/posts", data),

  deletePost: (slug: string): Promise<AxiosResponse<void>> => api.delete(`/api/v1/posts/${slug}`),
};
