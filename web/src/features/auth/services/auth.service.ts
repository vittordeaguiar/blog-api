import { api } from "@/shared/lib/axios";
import type { LoginRequest, RegisterRequest, AuthResponse } from "../types/auth.types";

export const authService = {
  login: async (credentials: LoginRequest) => {
    const { data } = await api.post<AuthResponse>("/v1/auth/login", credentials);
    return data;
  },

  register: async (userData: RegisterRequest) => {
    const { data } = await api.post<AuthResponse>("/v1/auth/register", userData);
    return data;
  },
};
