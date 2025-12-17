import { Link } from "react-router";
import { BookOpen } from "lucide-react";
import LoginForm from "../components/LoginForm";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/shared/ui/card";

export default function LoginPage() {
  return (
    <div className="min-h-screen grid lg:grid-cols-2">
      {/* Left Side - Gradient Background (Desktop only) */}
      <div className="hidden lg:flex flex-col items-center justify-center p-12 bg-gradient-to-br from-brand/20 via-purple-500/10 to-pink-500/10 relative overflow-hidden">
        {/* Pattern Background */}
        <div
          className="absolute inset-0 opacity-[0.03]"
          style={{
            backgroundImage: `linear-gradient(to right, oklch(0.21 0.006 285.885) 1px, transparent 1px), linear-gradient(to bottom, oklch(0.21 0.006 285.885) 1px, transparent 1px)`,
            backgroundSize: '4rem 4rem'
          }}
        />

        {/* Content */}
        <div className="relative z-10 max-w-md text-center space-y-6">
          <div className="inline-flex items-center gap-3 mb-8">
            <BookOpen className="size-12 text-primary" />
            <span className="font-display text-3xl font-bold">Blog API</span>
          </div>

          <h2 className="font-display text-4xl font-bold">
            Bem-vindo de volta!
          </h2>
          <p className="text-lg text-muted-foreground">
            Faça login para acessar sua conta e gerenciar seus posts
          </p>

          {/* Decorative gradient orb */}
          <div className="absolute top-1/4 -left-20 w-64 h-64 bg-gradient-to-r from-brand/20 to-purple-500/20 rounded-full blur-3xl" />
          <div className="absolute bottom-1/4 -right-20 w-64 h-64 bg-gradient-to-r from-pink-500/20 to-purple-500/20 rounded-full blur-3xl" />
        </div>
      </div>

      {/* Right Side - Login Form */}
      <div className="flex items-center justify-center p-6 sm:p-8 lg:p-12">
        <div className="w-full max-w-md space-y-8">
          {/* Mobile Logo */}
          <div className="lg:hidden flex flex-col items-center text-center space-y-2">
            <div className="inline-flex items-center gap-2">
              <BookOpen className="size-8 text-primary" />
              <span className="font-display text-2xl font-bold">Blog API</span>
            </div>
          </div>

          <Card className="border-border/50 shadow-xl">
            <CardHeader className="space-y-1">
              <CardTitle className="text-2xl font-display">Login</CardTitle>
              <CardDescription>
                Entre com seu email e senha para acessar sua conta
              </CardDescription>
            </CardHeader>
            <CardContent>
              <LoginForm />

              <div className="mt-6 text-center text-sm text-muted-foreground">
                Não tem uma conta?{" "}
                <Link
                  to="/registro"
                  className="font-medium text-primary hover:underline"
                >
                  Criar conta
                </Link>
              </div>
            </CardContent>
          </Card>

          <p className="text-center text-xs text-muted-foreground">
            Ao continuar, você concorda com nossos{" "}
            <Link to="/termos" className="underline hover:text-foreground">
              Termos de Serviço
            </Link>{" "}
            e{" "}
            <Link to="/privacidade" className="underline hover:text-foreground">
              Política de Privacidade
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
