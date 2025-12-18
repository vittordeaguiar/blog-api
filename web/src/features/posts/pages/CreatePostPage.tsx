import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { PostForm } from "../components/PostForm";
import { postsService } from "../services/posts.service";
import { categoriesService } from "@/features/categories/services/categories.service";
import type { Category } from "@/features/categories/types/category.types";

export default function CreatePostPage() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState<Category[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await categoriesService.getAll();
        setCategories(response.data);
      } catch (error) {
        toast.error("Failed to load categories");
        console.error(error);
      }
    };
    fetchCategories();
  }, []);

  const handleSubmit = async (data) => {
    setIsLoading(true);
    try {
      const response = await postsService.createPost(data);
      toast.success("Post criado com sucesso!");
      navigate(`/posts/${response.data.slug}`);
    } catch (error) {
      toast.error("Falha ao criar o post. Entre em contato com o suporte.");
      console.error(error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="container max-w-3xl mx-auto py-10 px-4">
      <h1 className="text-3xl font-bold mb-8 text-brand-900 dark:text-brand-50">Criar Post</h1>
      <PostForm categories={categories} onSubmit={handleSubmit} isLoading={isLoading} />
    </div>
  );
}
