import { useState } from "react";
import { useLogin } from "../hooks/useAuth";
import { Button } from "@/shared/ui/button";

export default function LoginForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const login = useLogin();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    login.mutate({ email, password });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label className="block text-sm font-medium mb-2">Email</label>
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full px-3 py-2 border rounded-md"
          required
        />
      </div>

      <div>
        <label className="block text-sm font-medium mb-2">Senha</label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="w-full px-3 py-2 border rounded-md"
          required
        />
      </div>

      {login.isError && (
        <p className="text-red-600 text-sm">Erro ao fazer login. Verifique suas credenciais.</p>
      )}

      <Button type="submit" disabled={login.isPending} className="w-full">
        {login.isPending ? "Entrando..." : "Entrar"}
      </Button>
    </form>
  );
}
