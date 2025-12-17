import { Link } from "react-router";
import { Button } from "@/shared/ui/button";

export function Hero() {
  return (
    <section className="relative min-h-[500px] flex items-center justify-center overflow-hidden">
      {/* Background animado com gradientes */}
      <div className="absolute inset-0 bg-gradient-to-br from-brand/10 via-purple-500/10 to-pink-500/10" />

      {/* Padrão de grade no fundo */}
      <div
        className="absolute inset-0 opacity-[0.02]"
        style={{
          backgroundImage: `linear-gradient(to right, oklch(0.21 0.006 285.885) 1px, transparent 1px), linear-gradient(to bottom, oklch(0.21 0.006 285.885) 1px, transparent 1px)`,
          backgroundSize: '4rem 4rem'
        }}
      />

      {/* Content */}
      <div className="container relative z-10 mx-auto px-4">
        <div className="max-w-4xl mx-auto text-center space-y-6">
          <h1 className="font-display text-4xl md:text-5xl lg:text-6xl font-bold tracking-tight">
            Descubra Histórias Que{" "}
            <span className="bg-gradient-to-r from-brand to-purple-600 bg-clip-text text-transparent">
              Importam
            </span>
          </h1>
          <p className="text-lg md:text-xl text-muted-foreground max-w-2xl mx-auto leading-relaxed">
            Explore artigos, insights e ideias de escritores ao redor do mundo
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center pt-4">
            <Button size="lg" asChild>
              <Link to="/#posts">Começar a Ler</Link>
            </Button>
            <Button size="lg" variant="outline" asChild>
              <Link to="/login">Criar Conta</Link>
            </Button>
          </div>
        </div>
      </div>
    </section>
  );
}
