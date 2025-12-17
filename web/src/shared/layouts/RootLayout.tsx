import { Outlet } from "react-router-dom";

export default function RootLayout() {
  return (
    <div className="min-h-screen flex flex-col">
      <header className="border-b p-4">
        <h1 className="text-2xl font-bold">Blog API</h1>
      </header>

      <main className="flex-1 container mx-auto py-8">
        <Outlet />
      </main>

      <footer className="border-t p-4 text-center text-sm text-zinc-500">© 2025 Blog API</footer>
    </div>
  );
}
