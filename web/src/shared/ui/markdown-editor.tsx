import * as React from "react";
import MDEditor from "@uiw/react-md-editor";
import { cn } from "@/shared/lib/utils";
import { useTheme } from "@/shared/providers/ThemeProvider";

interface MarkdownEditorProps {
  value?: string;
  onChange?: (e: React.ChangeEvent<HTMLTextAreaElement>) => void;
  placeholder?: string;
  className?: string;
}

export function MarkdownEditor({
  className,
  value = "",
  onChange,
  placeholder,
  ...props
}: MarkdownEditorProps) {
  const { theme } = useTheme();

  // Adapter: converte onChange do MDEditor para o formato esperado pelo React Hook Form
  const handleChange = React.useCallback((val?: string) => {
    if (!onChange) return;

    // Cria um evento sintético compatível com React Hook Form
    const event = {
      target: {
        value: val || "",
        name: "content", // nome do campo no form
      },
    } as React.ChangeEvent<HTMLTextAreaElement>;

    onChange(event);
  }, [onChange]);

  // Resolve tema: "system" -> "auto", senão usa o tema atual
  const colorMode = theme === "system" ? "auto" : theme;

  return (
    <div className={cn("markdown-editor-wrapper", className)} data-color-mode={colorMode}>
      <MDEditor
        value={value}
        onChange={handleChange}
        preview="edit"
        height={400}
        textareaProps={{
          placeholder: placeholder || "Write your post content in Markdown...",
        }}
      />
    </div>
  );
}
