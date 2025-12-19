import { FileText, Search, AlertCircle } from "lucide-react";
import { motion } from "framer-motion";
import { Button } from "@/shared/ui/button";
import { Link } from "react-router";

interface EmptyStateProps {
  variant?: "no-posts" | "no-search-results" | "error";
}

const variantConfig = {
  "no-posts": {
    icon: FileText,
    title: "Nenhum post disponível",
    description: "Volte mais tarde para ver novos conteúdos",
    showCTA: false,
  },
  "no-search-results": {
    icon: Search,
    title: "Nenhum resultado encontrado",
    description: "Tente ajustar os termos de busca ou filtros",
    showCTA: true,
    ctaText: "Limpar filtros",
    ctaAction: "clear-filters",
  },
  error: {
    icon: AlertCircle,
    title: "Algo deu errado",
    description: "Não foi possível carregar os posts. Tente novamente.",
    showCTA: true,
    ctaText: "Tentar novamente",
    ctaAction: "reload",
  },
};

export default function EmptyState({ variant = "no-posts" }: EmptyStateProps) {
  const config = variantConfig[variant];
  const Icon = config.icon;

  const handleAction = () => {
    if (config.ctaAction === "reload") {
      window.location.reload();
    } else if (config.ctaAction === "clear-filters") {
      window.location.href = "/";
    }
  };

  return (
    <motion.div
      className="flex flex-col items-center justify-center min-h-[400px] text-center px-4"
      initial={{ opacity: 0, scale: 0.9 }}
      animate={{ opacity: 1, scale: 1 }}
      transition={{ duration: 0.4, ease: "easeOut" }}
    >
      <motion.div
        initial={{ scale: 0 }}
        animate={{ scale: 1 }}
        transition={{ delay: 0.2, type: "spring", stiffness: 200 }}
      >
        <Icon className="size-20 text-muted-foreground mb-4" />
      </motion.div>

      <motion.h2
        className="text-2xl font-semibold mb-2"
        initial={{ opacity: 0, y: 10 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.3 }}
      >
        {config.title}
      </motion.h2>

      <motion.p
        className="text-muted-foreground mb-6"
        initial={{ opacity: 0, y: 10 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.4 }}
      >
        {config.description}
      </motion.p>

      {config.showCTA && (
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.5 }}
        >
          <Button onClick={handleAction} variant="default">
            {config.ctaText}
          </Button>
        </motion.div>
      )}
    </motion.div>
  );
}
