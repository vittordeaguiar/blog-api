import * as React from "react";
import { Check, Plus } from "lucide-react";
import { Button } from "@/shared/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/shared/ui/dialog";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { categoriesService } from "../services/categories.service";
import type { Category } from "../types/category.types";
import { toast } from "sonner";
import { cn } from "@/shared/lib/utils";

interface CategorySelectorProps {
  selectedIds: string[];
  onSelectionChange: (ids: string[]) => void;
  categories: Category[];
  onCategoryCreated: (newCategory: Category) => void;
}

export function CategorySelector({
  selectedIds,
  onSelectionChange,
  categories,
  onCategoryCreated,
}: CategorySelectorProps) {
  const [isCreating, setIsCreating] = React.useState(false);
  const [newCategoryName, setNewCategoryName] = React.useState("");
  const [newCategoryDesc, setNewCategoryDesc] = React.useState("");
  const [isDialogOpen, setIsDialogOpen] = React.useState(false);

  const handleCreateCategory = async () => {
    if (!newCategoryName.trim()) return;

    setIsCreating(true);
    try {
      const response = await categoriesService.createCategory({
        name: newCategoryName,
        description: newCategoryDesc || newCategoryName,
      });
      onCategoryCreated(response.data);
      setNewCategoryName("");
      setNewCategoryDesc("");
      setIsDialogOpen(false);
      toast.success("Category created successfully");
    } catch (error) {
      console.error(error);
      toast.error("Failed to create category");
    } finally {
      setIsCreating(false);
    }
  };

  const toggleSelection = (id: string) => {
    if (selectedIds.includes(id)) {
      onSelectionChange(selectedIds.filter((item) => item !== id));
    } else {
      onSelectionChange([...selectedIds, id]);
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap gap-2">
        {categories.map((category) => {
          const isSelected = selectedIds.includes(category.id);
          return (
            <button
              key={category.id}
              type="button"
              onClick={() => toggleSelection(category.id)}
              className={cn(
                "cursor-pointer inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-semibold transition-colors focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2",
                isSelected
                  ? "border-transparent bg-primary text-primary-foreground hover:bg-primary/80"
                  : "border-transparent bg-secondary text-secondary-foreground hover:bg-secondary/80"
              )}
            >
              {category.name}
              {isSelected && <Check className="ml-1 h-3 w-3" />}
            </button>
          );
        })}

        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogTrigger asChild>
            <Button variant="outline" size="sm" className="h-6 rounded-full text-xs cursor-pointer">
              <Plus className="mr-1 h-3 w-3" />
              Nova categoria
            </Button>
          </DialogTrigger>
          <DialogContent className="sm:max-w-106.25">
            <DialogHeader>
              <DialogTitle>Criar categoria</DialogTitle>
              <DialogDescription>Adicione uma nova categoria.</DialogDescription>
            </DialogHeader>
            <div className="grid gap-4 py-4">
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="name" className="text-right">
                  Name
                </Label>
                <Input
                  id="name"
                  value={newCategoryName}
                  onChange={(e) => setNewCategoryName(e.target.value)}
                  className="col-span-3"
                  placeholder="Tecnologia, Esportes..."
                />
              </div>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="description" className="text-right">
                  Descrição
                </Label>
                <Input
                  id="description"
                  value={newCategoryDesc}
                  onChange={(e) => setNewCategoryDesc(e.target.value)}
                  className="col-span-3"
                  placeholder="Breve descrição"
                />
              </div>
            </div>
            <DialogFooter>
              <Button
                onClick={handleCreateCategory}
                disabled={isCreating}
                className="cursor-pointer"
              >
                {isCreating ? "Criando..." : "Criar"}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>
      {selectedIds.length === 0 && (
        <p className="text-sm text-muted-foreground">Nenhuma categoria selecionada.</p>
      )}
    </div>
  );
}
