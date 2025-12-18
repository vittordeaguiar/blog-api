import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Button } from "@/shared/ui/button";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/shared/ui/form";
import { Input } from "@/shared/ui/input";
import { MarkdownEditor } from "@/shared/ui/markdown-editor";
import { CategorySelector } from "@/features/categories/components/CategorySelector";
import type { Category } from "@/features/categories/types/category.types";
import { useEffect, useState } from "react";

const formSchema = z.object({
  title: z.string().min(3, "O título deve ter pelo menos 3 caracteres"),
  content: z.string().min(10, "O conteúdo deve ter pelo menos 10 caracteres"),
  categoryIds: z.array(z.string()).optional(),
});

type PostFormValues = z.infer<typeof formSchema>;

interface PostFormProps {
  initialData?: Partial<PostFormValues>;
  categories: Category[];
  onSubmit: (data: PostFormValues) => void;
  isLoading?: boolean;
}

export function PostForm({
  initialData,
  categories: initialCategories,
  onSubmit,
  isLoading,
}: PostFormProps) {
  const [localCategories, setLocalCategories] = useState(initialCategories);

  useEffect(() => {
    setLocalCategories(initialCategories);
  }, [initialCategories]);

  const form = useForm<PostFormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      title: initialData?.title || "",
      content: initialData?.content || "",
      categoryIds: initialData?.categoryIds || [],
    },
  });

  const handleCategoryCreated = (newCategory: Category) => {
    setLocalCategories((prev) => [...prev, newCategory]);

    const currentIds = form.getValues("categoryIds") || [];
    form.setValue("categoryIds", [...currentIds, newCategory.id]);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <FormField
          control={form.control}
          name="title"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Title</FormLabel>
              <FormControl>
                <Input placeholder="Insira o título do post..." {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="categoryIds"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Categorias</FormLabel>
              <FormControl>
                <CategorySelector
                  categories={localCategories}
                  selectedIds={field.value || []}
                  onSelectionChange={field.onChange}
                  onCategoryCreated={handleCategoryCreated}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="content"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Conteúdo</FormLabel>
              <FormControl>
                <MarkdownEditor
                  placeholder="Escreva o conteúdo do seu post utilizando Markdown!..."
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <div className="flex justify-end">
          <Button type="submit" disabled={isLoading} className="cursor-pointer">
            {isLoading ? "Salvando..." : "Criar"}
          </Button>
        </div>
      </form>
    </Form>
  );
}
