import { useSearchParams } from "react-router";
import { usePosts } from "../hooks/usePost";
import PostCard from "../components/PostCard";
import PostCardSkeleton from "../components/PostCardSkeleton";
import EmptyState from "../components/EmptyState";
import Pagination from "../components/Pagination";
import { Hero } from "@/shared/components/Hero";

export default function PostsListPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const currentPage = parseInt(searchParams.get("page") || "1", 10);

  const handlePageChange = (newPage: number) => {
    setSearchParams({ page: newPage.toString() });
  };

  const { data, isLoading, error } = usePosts({
    page: currentPage,
    pageSize: 9,
  });

  return (
    <>
      <Hero />

      <div id="posts" className="container mx-auto py-12 px-4">
        <h2 className="text-3xl font-display font-bold mb-8">Posts Recentes</h2>

      {isLoading && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {Array.from({ length: 9 }).map((_, i) => (
            <PostCardSkeleton key={i} />
          ))}
        </div>
      )}

      {error && (
        <div className="bg-destructive/10 border border-destructive/20 rounded-lg p-6 text-center">
          <p className="text-destructive font-semibold">Erro ao carregar posts</p>
          <p className="text-muted-foreground mt-2">{error.message}</p>
        </div>
      )}

      {data && data.items.length === 0 && <EmptyState />}

      {data && data.items.length > 0 && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {data.items.map((post) => (
              <PostCard key={post.id} post={post} />
            ))}
          </div>

          {data.totalPages > 1 && (
            <div className="mt-8">
              <Pagination
                currentPage={data.page}
                totalPages={data.totalPages}
                onPageChange={handlePageChange}
              />
            </div>
          )}
        </>
      )}
      </div>
    </>
  );
}
