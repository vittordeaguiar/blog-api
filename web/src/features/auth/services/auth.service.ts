import { api } from "@/shared/lib/axios";

interface LoginRequest {
  email: string;
  password: string;
}

interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  role: "Author" | "Admin";
}

interface AuthResponse {
  token: string;
}

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
