import { Link } from "react-router";
import { Button } from "@/shared/ui/button";
import { useAuthStore } from "@/features/auth/stores/authStore";

export function Hero() {
  const { isAuthenticated, user } = useAuthStore();

  return (
    <section className="relative min-h-[500px] flex items-center justify-center overflow-hidden border-b border-border/40">
      {/* Background Gradients */}
      <div className="absolute inset-0 bg-background" />
      <div className="absolute inset-0 bg-linear-to-b from-primary/5 via-background to-background" />
      <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[800px] h-[500px] bg-brand/10 blur-[100px] rounded-full pointer-events-none" />

      {/* Grid Pattern */}
      <div
        className="absolute inset-0 opacity-[0.03]"
        style={{
          backgroundImage: `linear-gradient(to right, currentColor 1px, transparent 1px), linear-gradient(to bottom, currentColor 1px, transparent 1px)`,
          backgroundSize: "3rem 3rem",
        }}
      />

      <div className="container relative z-10 mx-auto px-4">
        <div className="max-w-3xl mx-auto text-center space-y-8">
          {isAuthenticated ? (
            <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-700">
              <span className="inline-block px-3 py-1 rounded-full bg-brand/10 text-brand text-sm font-medium border border-brand/20">
                Bem-vindo de volta
              </span>
              <h1 className="font-display text-4xl md:text-5xl lg:text-6xl font-bold tracking-tight text-foreground">
                Olá,{" "}
                <span className="text-brand">
                  {user?.name?.split(" ")[0] || "Visitante"}
                </span>
              </h1>
              <p className="text-lg md:text-xl text-muted-foreground max-w-2xl mx-auto leading-relaxed">
                Pronto para compartilhar suas ideias hoje? O mundo está esperando por sua próxima história.
              </p>
              <div className="flex flex-col sm:flex-row gap-4 justify-center pt-2">
                <Button size="lg" className="h-12 px-8 text-base shadow-lg shadow-brand/20" asChild>
                  <Link to="/posts/new">Criar Novo Post</Link>
                </Button>
                <Button size="lg" variant="outline" className="h-12 px-8 text-base bg-background/50 backdrop-blur-sm" asChild>
                  <Link to="/#posts">Explorar Feed</Link>
                </Button>
              </div>
            </div>
          ) : (
            <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-700">
               <span className="inline-block px-3 py-1 rounded-full bg-primary/10 text-primary text-sm font-medium border border-primary/20">
                Blog API v2
              </span>
              <h1 className="font-display text-4xl md:text-6xl lg:text-7xl font-bold tracking-tight text-foreground">
                Ideias que <br className="hidden md:block" />
                <span className="text-brand inline-block mt-2">Transformam</span>
              </h1>
              <p className="text-lg md:text-xl text-muted-foreground max-w-2xl mx-auto leading-relaxed">
                Uma comunidade vibrante de escritores e pensadores. Junte-se a nós para ler, escrever e se conectar.
              </p>
              <div className="flex flex-col sm:flex-row gap-4 justify-center pt-4">
                <Button size="lg" className="h-12 px-8 text-base shadow-lg shadow-primary/20" asChild>
                  <Link to="/login">Começar Agora</Link>
                </Button>
                <Button size="lg" variant="outline" className="h-12 px-8 text-base bg-background/50 backdrop-blur-sm" asChild>
                  <Link to="/#posts">Ler Artigos</Link>
                </Button>
              </div>
            </div>
          )}
        </div>
      </div>
    </section>
  );
}
