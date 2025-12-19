import { Link } from "react-router";
import { User, Calendar, ArrowRight } from "lucide-react";
import { motion } from "framer-motion";
import { useInView } from "react-intersection-observer";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import { Button } from "@/shared/ui/button";
import type { Post } from "../types/post.types";
import { truncate } from "@/shared/lib/utils";
import { formatRelativeTime } from "@/shared/lib/date";

interface PostCardProps {
  post: Post;
}

export default function PostCard({ post }: PostCardProps) {
  const visibleCategories = post.categories.slice(0, 3);
  const remainingCount = post.categories.length - 3;

  const { ref, inView } = useInView({
    triggerOnce: true,
    threshold: 0.1,
  });

  return (
    <motion.article
      ref={ref}
      className="group h-full"
      initial={{ opacity: 0, y: 20 }}
      animate={inView ? { opacity: 1, y: 0 } : { opacity: 0, y: 20 }}
      transition={{ duration: 0.4, ease: "easeOut" }}
    >
      <Card className="relative overflow-hidden hover:shadow-lg hover:-translate-y-1 transition-all duration-300 border border-border/50 bg-card/80 backdrop-blur-sm h-full flex flex-col">
        {/* Hover Gradient Background */}
        <div className="absolute inset-0 bg-linear-to-br from-brand/5 to-primary/5 opacity-0 group-hover:opacity-100 transition-opacity duration-300" />

        {/* Accent Border Left */}
        <div className="absolute top-0 left-0 w-1 h-full bg-brand/80" />

        <div className="relative flex-1 flex flex-col">
          <CardHeader>
            <CardTitle className="text-xl font-display font-bold leading-tight line-clamp-2 group-hover:text-brand transition-colors duration-300">
              {post.title}
            </CardTitle>
            <CardDescription className="line-clamp-3 text-base mt-2">
              {truncate(post.content, 140)}
            </CardDescription>
          </CardHeader>

          <CardContent className="space-y-4 flex-1">
            <div className="space-y-2 text-sm text-muted-foreground">
              <div className="flex items-center gap-2">
                <User className="size-4 text-brand/70" />
                <span className="font-medium">{post.authorName}</span>
              </div>
              <div className="flex items-center gap-2">
                <Calendar className="size-4 text-brand/70" />
                <span>{formatRelativeTime(post.publishedAt || post.createdAt)}</span>
              </div>
            </div>

            {post.categories.length > 0 && (
              <div className="flex flex-wrap gap-2 pt-2">
                {visibleCategories.map((category) => (
                  <Badge
                    key={category.id}
                    variant="secondary"
                    className="transition-colors group-hover:bg-brand/10 group-hover:text-brand border-transparent"
                  >
                    {category.name}
                  </Badge>
                ))}
                {remainingCount > 0 && (
                  <Badge
                    variant="secondary"
                    className="transition-colors group-hover:bg-brand/10 group-hover:text-brand border-transparent"
                  >
                    +{remainingCount}
                  </Badge>
                )}
              </div>
            )}
          </CardContent>

          <CardFooter>
            <Button
              asChild
              variant="ghost"
              className="w-full justify-between group-hover:bg-brand/10 group-hover:text-brand transition-colors pl-0 hover:pl-4"
            >
              <Link to={`/posts/${post.slug}`}>
                Ler mais
                <ArrowRight className="ml-2 size-4 group-hover:translate-x-1 transition-transform" />
              </Link>
            </Button>
          </CardFooter>
        </div>
      </Card>
    </motion.article>
  );
}
