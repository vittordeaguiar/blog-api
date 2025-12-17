export interface Category {
  id: string;
  name: string;
  slug: string;
}

export interface Post {
  id: string;
  title: string;
  content: string;
  slug: string;
  authorId: string;
  authorName: string;
  createdAt: string;
  updatedAt: string | null;
  publishedAt: string | null;
  isPublished: boolean;
  categories: Category[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface PostsQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  categoryId?: string;
}
