export interface Category {
  id: string;
  name: string;
  slug: string;
}

export interface CreateCategoryRequest {
  name: string;
  description: string;
}