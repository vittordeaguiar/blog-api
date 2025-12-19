import { useSearchParams } from "react-router";
import { motion } from "framer-motion";
import { usePosts } from "../hooks/usePost";
import PostCard from "../components/PostCard";
import PostCardSkeleton from "../components/PostCardSkeleton";
import EmptyState from "../components/EmptyState";
import Pagination from "../components/Pagination";
import { Hero } from "@/shared/components/Hero";
import { containerVariants, itemVariants } from "@/shared/lib/animations";

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

      <section id="posts" className="py-16 bg-muted/30">
        <div className="container mx-auto px-4">
          <div className="flex flex-col md:flex-row justify-between items-end mb-10 gap-4">
            <div>
              <h2 className="text-3xl font-display font-bold text-foreground">
                Publicações Recentes
              </h2>
              <p className="text-muted-foreground mt-2 max-w-xl">
                Fique por dentro das últimas novidades, tutoriais e artigos da nossa comunidade.
              </p>
            </div>
            {/* Future: Add Sort/Filter Controls here */}
          </div>

          {isLoading && (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
              {Array.from({ length: 9 }).map((_, i) => (
                <PostCardSkeleton key={i} />
              ))}
            </div>
          )}

          {error && (
            <div className="bg-destructive/5 border border-destructive/20 rounded-xl p-8 text-center max-w-2xl mx-auto">
              <p className="text-destructive font-semibold text-lg">Erro ao carregar publicações</p>
              <p className="text-muted-foreground mt-2">{error.message}</p>
            </div>
          )}

          {data && data.items.length === 0 && (
            <div className="py-12">
               <EmptyState />
            </div>
          )}

          {data && data.items.length > 0 && (
            <div className="space-y-12">
              <motion.div
                className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8"
                variants={containerVariants}
                initial="hidden"
                animate="show"
              >
                {data.items.map((post) => (
                  <motion.div key={post.id} variants={itemVariants}>
                    <PostCard post={post} />
                  </motion.div>
                ))}
              </motion.div>

              {data.totalPages > 1 && (
                <div className="flex justify-center pt-8 border-t border-border">
                  <Pagination
                    currentPage={data.page}
                    totalPages={data.totalPages}
                    onPageChange={handlePageChange}
                  />
                </div>
              )}
            </div>
          )}
        </div>
      </section>
    </>
  );
}
