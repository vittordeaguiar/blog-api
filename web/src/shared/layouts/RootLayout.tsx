import { Outlet } from "react-router-dom";
import { Header } from "./components/Header";

export default function RootLayout() {
  return (
    <div className="min-h-screen flex flex-col">
      <a href="#main-content" className="skip-link">
        Pular para o conteúdo principal
      </a>

      <Header />

      <main id="main-content" className="flex-1">
        <Outlet />
      </main>

      <footer className="border-t p-4 text-center text-sm text-muted-foreground">
        © 2025 Blog API
      </footer>
    </div>
  );
}
