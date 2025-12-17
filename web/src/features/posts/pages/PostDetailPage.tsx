import { useParams, Link } from "react-router";
import { ArrowLeft, User, Calendar, Edit, Trash2, AlertCircle } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Badge } from "@/shared/ui/badge";
import { Separator } from "@/shared/ui/separator";
import { useAuthStore } from "@/features/auth/stores/authStore";
import { usePostDetail } from "../hooks/usePostDetail";
import { PostDetailSkeleton } from "../components/PostDetailSkeleton";
import { PostContent } from "../components/PostContent";
import { formatRelativeTime } from "@/shared/lib/date";

export default function PostDetailPage() {
  const { slug } = useParams<{ slug: string }>();
  const { post, isLoading, error, isDeleting, handleDelete } = usePostDetail(slug!);
  const { user } = useAuthStore();

  if (isLoading) {
    return (
      <div className="container mx-auto max-w-4xl py-8 px-4">
        <PostDetailSkeleton />
      </div>
    );
  }

  if (error) {
    const is404 = error?.response?.status === 404;

    return (
      <div className="container mx-auto max-w-4xl py-8 px-4">
        <div className="bg-destructive/10 border border-destructive/20 rounded-lg p-8 text-center space-y-4">
          <AlertCircle className="size-12 mx-auto text-destructive" />
          <h2 className="text-2xl font-semibold text-destructive">
            {is404 ? "Post não encontrado" : "Erro ao carregar post"}
          </h2>
          <p className="text-muted-foreground">
            {is404
              ? "O post que você procura não existe ou foi removido."
              : error.message}
          </p>
          <Button asChild>
            <Link to="/">
              <ArrowLeft className="mr-2 size-4" />
              Voltar para home
            </Link>
          </Button>
        </div>
      </div>
    );
  }

  if (!post) {
    return null;
  }

  const canEdit = user && (user.id === post.authorId || user.role === "Admin");

  return (
    <div className="container mx-auto max-w-4xl py-8 px-4">
      <div className="space-y-8">
        <Button asChild variant="ghost" size="sm">
          <Link to="/">
            <ArrowLeft className="mr-2 size-4" />
            Voltar
          </Link>
        </Button>

        <header className="space-y-4">
          <h1 className="text-2xl md:text-4xl font-bold leading-tight">
            {post.title}
          </h1>
          <div className="flex flex-col sm:flex-row gap-4 text-sm text-muted-foreground">
            <div className="flex items-center gap-2">
              <User className="size-4" />
              <span>{post.authorName}</span>
            </div>
            <div className="flex items-center gap-2">
              <Calendar className="size-4" />
              <span>{formatRelativeTime(post.publishedAt || post.createdAt)}</span>
            </div>
          </div>
          {post.categories.length > 0 && (
            <div className="flex flex-wrap gap-2">
              {post.categories.map((category) => (
                <Badge key={category.id} variant="secondary">
                  {category.name}
                </Badge>
              ))}
            </div>
          )}
        </header>

        <Separator />

        <PostContent content={post.content} />

        {canEdit && (
          <>
            <Separator />
            <div className="flex flex-col sm:flex-row gap-3">
              <Button asChild variant="outline" className="w-full sm:w-auto">
                <Link to={`/posts/${post.slug}/edit`}>
                  <Edit className="mr-2 size-4" />
                  Editar Post
                </Link>
              </Button>
              <Button
                variant="destructive"
                onClick={handleDelete}
                disabled={isDeleting}
                className="w-full sm:w-auto"
              >
                <Trash2 className="mr-2 size-4" />
                {isDeleting ? "Deletando..." : "Deletar Post"}
              </Button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
