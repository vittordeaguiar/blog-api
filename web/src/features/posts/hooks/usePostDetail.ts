import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router";
import { postsService } from "../services/posts.service";
import type { Post } from "../types/post.types";

export function usePostDetail(slug: string) {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const query = useQuery<Post>({
    queryKey: ["post", slug],
    queryFn: async () => {
      const { data } = await postsService.getPost(slug);
      return data;
    },
    retry: 1,
    staleTime: 5 * 60 * 1000,
  });

  const deleteMutation = useMutation({
    mutationFn: () => postsService.deletePost(slug),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["posts"] });
      navigate("/");
    },
  });

  const handleDelete = () => {
    const confirmed = window.confirm(
      "Tem certeza que deseja deletar este post? Esta ação não pode ser desfeita."
    );
    if (confirmed) {
      deleteMutation.mutate();
    }
  };

  return {
    post: query.data,
    isLoading: query.isLoading,
    error: query.error,
    isDeleting: deleteMutation.isPending,
    handleDelete,
  };
}
