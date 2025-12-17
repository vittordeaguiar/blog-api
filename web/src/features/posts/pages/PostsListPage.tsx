import { usePosts } from "../hooks/usePost";

export default function PostsListPage() {
  const { data, isLoading, error } = usePosts();

  if (isLoading) return <div>Carregando...</div>;
  if (error) return <div>Erro: {error.message}</div>;

  return (
    <div>
      <h1 className="text-3xl font-bold mb-4">Posts</h1>
      <pre className="bg-gray-100 p-4 rounded">{JSON.stringify(data, null, 2)}</pre>
    </div>
  );
}
