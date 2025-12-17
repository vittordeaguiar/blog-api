import { QueryClient } from "@tanstack/react-query";

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutos
      gcTime: 1000 * 60 * 10, // 10 minutos
      retry: 1, // Tentar 1 vez se falhar
      refetchOnWindowFocus: false, // Não buscar ao focar na janela
    },
  },
});
