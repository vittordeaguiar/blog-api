export function formatRelativeTime(date: string): string {
  const now = new Date();
  const past = new Date(date);
  const diffInMs = now.getTime() - past.getTime();
  const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

  const rtf = new Intl.RelativeTimeFormat("pt-BR", { numeric: "auto" });

  if (diffInDays === 0) {
    return "hoje";
  }

  if (diffInDays === 1) {
    return "ontem";
  }

  if (diffInDays < 7) {
    return rtf.format(-diffInDays, "day");
  }

  if (diffInDays < 30) {
    const weeks = Math.floor(diffInDays / 7);
    return rtf.format(-weeks, "week");
  }

  if (diffInDays < 365) {
    const months = Math.floor(diffInDays / 30);
    return rtf.format(-months, "month");
  }

  const years = Math.floor(diffInDays / 365);
  return rtf.format(-years, "year");
}
