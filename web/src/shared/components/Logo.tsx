import { Link } from "react-router";
import { BookOpen } from "lucide-react";

export function Logo() {
  return (
    <Link
      to="/"
      className="flex items-center gap-2 group transition-transform hover:scale-105"
    >
      <BookOpen className="size-8 text-primary" />
      <span className="font-display text-xl font-bold">Blog API</span>
    </Link>
  );
}
