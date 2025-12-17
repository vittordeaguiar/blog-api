import { FileText } from "lucide-react";

export default function EmptyState() {
  return (
    <div className="flex flex-col items-center justify-center min-h-[400px] text-center">
      <FileText className="size-20 text-muted-foreground mb-4" />
      <h2 className="text-2xl font-semibold mb-2">Nenhum post disponível</h2>
      <p className="text-muted-foreground">
        Volte mais tarde para ver novos conteúdos
      </p>
    </div>
  );
}
