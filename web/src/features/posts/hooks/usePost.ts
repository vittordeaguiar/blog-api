import { useQuery } from "@tanstack/react-query";
import { postsService } from "../services/posts.service";
import type { PostsQueryParams, PagedResult, Post } from "../types/post.types";

export function usePosts(params?: PostsQueryParams) {
  return useQuery<PagedResult<Post>>({
    queryKey: ["posts", params],
    queryFn: async () => {
      const { data } = await postsService.getPosts(params);
      return data;
    },
  });
}
