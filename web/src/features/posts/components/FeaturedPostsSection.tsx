import { motion } from "framer-motion";
import { Sparkles } from "lucide-react";
import { usePosts } from "../hooks/usePost";
import PostCard from "./PostCard";
import PostCardSkeleton from "./PostCardSkeleton";
import { containerVariants, itemVariants } from "@/shared/lib/animations";

export function FeaturedPostsSection() {
  const { data, isLoading, error } = usePosts({
    page: 1,
    pageSize: 3,
  });

  if (error) {
    return null;
  }

  return (
    <section className="py-16 bg-muted/30">
      <div className="container mx-auto px-4">
        <motion.div
          className="flex items-center gap-2 mb-8"
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          transition={{ duration: 0.5 }}
        >
          <Sparkles className="h-6 w-6 text-brand" />
          <h2 className="text-3xl font-display font-bold text-foreground">
            Posts em Destaque
          </h2>
        </motion.div>

        {isLoading && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {Array.from({ length: 3 }).map((_, i) => (
              <PostCardSkeleton key={i} />
            ))}
          </div>
        )}

        {data && data.items.length > 0 && (
          <motion.div
            className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8"
            variants={containerVariants}
            initial="hidden"
            whileInView="show"
            viewport={{ once: true, amount: 0.2 }}
          >
            {data.items.map((post) => (
              <motion.div key={post.id} variants={itemVariants}>
                <PostCard post={post} />
              </motion.div>
            ))}
          </motion.div>
        )}
      </div>
    </section>
  );
}
