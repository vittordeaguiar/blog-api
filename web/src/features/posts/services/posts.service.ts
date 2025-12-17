export const postsService = {
  getPosts: (params) => api.get("/api/v1/posts", { params }),
  getPost: (slug: string) => api.get(`/api/v1/posts/${slug}`),
  createPost: (data) => api.post("/api/v1/posts", data),
};
