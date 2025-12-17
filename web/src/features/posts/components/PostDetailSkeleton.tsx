import { Skeleton } from "@/shared/ui/skeleton";
import { Separator } from "@/shared/ui/separator";

export function PostDetailSkeleton() {
  return (
    <div className="space-y-8">
      <Skeleton className="h-4 w-32" />

      <div className="space-y-4">
        <Skeleton className="h-10 w-3/4" />
        <div className="flex flex-col sm:flex-row gap-4 text-sm">
          <Skeleton className="h-4 w-40" />
          <Skeleton className="h-4 w-32" />
        </div>
        <div className="flex gap-2">
          <Skeleton className="h-6 w-20" />
          <Skeleton className="h-6 w-24" />
          <Skeleton className="h-6 w-20" />
        </div>
      </div>

      <Separator />

      <div className="space-y-3">
        {Array.from({ length: 10 }).map((_, i) => (
          <Skeleton
            key={i}
            className="h-4"
            style={{ width: `${Math.random() * 30 + 70}%` }}
          />
        ))}
      </div>
    </div>
  );
}
