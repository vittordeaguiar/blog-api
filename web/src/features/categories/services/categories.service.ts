import type { AxiosResponse } from "axios";
import { api } from "@/shared/lib/axios";
import type { Category, CreateCategoryRequest } from "../types/category.types";

export const categoriesService = {
  getAll: (): Promise<AxiosResponse<Category[]>> => api.get("/v1/categories"),
  createCategory: (data: CreateCategoryRequest): Promise<AxiosResponse<Category>> =>
    api.post("/v1/categories", data),
};