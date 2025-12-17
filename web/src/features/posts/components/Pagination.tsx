import { ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "@/shared/ui/button";

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export default function Pagination({
  currentPage,
  totalPages,
  onPageChange,
}: PaginationProps) {
  return (
    <nav className="flex items-center justify-center gap-4">
      <Button
        variant="outline"
        size="sm"
        disabled={currentPage === 1}
        onClick={() => onPageChange(currentPage - 1)}
        aria-label="Página anterior"
      >
        <ChevronLeft className="size-4 mr-2" />
        Anterior
      </Button>
      <span className="text-sm font-medium">
        Página {currentPage} de {totalPages}
      </span>
      <Button
        variant="outline"
        size="sm"
        disabled={currentPage === totalPages}
        onClick={() => onPageChange(currentPage + 1)}
        aria-label="Próxima página"
      >
        Próximo
        <ChevronRight className="size-4 ml-2" />
      </Button>
    </nav>
  );
}
