export interface User {
  id: string;
  name: string;
  email: string;
  role: "Author" | "Admin";
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  role: "Author" | "Admin";
}

export interface AuthResponse {
  username: string;
  token: string;
}
