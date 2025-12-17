import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function truncate(text: string, length: number = 150): string {
  if (text.length <= length) {
    return text;
  }
  return text.substring(0, length).trim() + "...";
}
