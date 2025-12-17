import { useQuery } from "@tanstack/react-query";
import { api } from "@/shared/lib/axios";

export function usePosts() {
  return useQuery({
    queryKey: ["posts"],
    queryFn: async () => {
      const { data } = await api.get("/api/v1/posts");
      return data;
    },
  });
}
