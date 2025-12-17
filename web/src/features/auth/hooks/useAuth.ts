import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/auth.service";
import { useAuthStore } from "../stores/authStore";
import type { User } from "../types/auth.types";

function decodeToken(token: string): User | null {
  try {
    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split("")
        .map(function (c) {
          return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join("")
    );

    const payload = JSON.parse(jsonPayload);

    return {
      id: payload.nameid || payload.sub || payload.userId,
      name: payload.unique_name || payload.name,
      email: payload.email,
      role: payload.role,
    };
  } catch (error) {
    console.error("Token decoding failed:", error);
    return null;
  }
}

export function useLogin() {
  const setAuth = useAuthStore((state) => state.setAuth);
  const navigate = useNavigate();

  return useMutation({
    mutationFn: authService.login,
    onSuccess: (data) => {
      const user = decodeToken(data.token);
      if (user) {
        // If the API returns a username explicitly, we could prioritize it,
        // but the token's unique_name should match.
        if (data.username) {
          user.name = data.username;
        }
        setAuth(data.token, user);
        navigate("/");
      }
    },
  });
}

export function useRegister() {
  const setAuth = useAuthStore((state) => state.setAuth);
  const navigate = useNavigate();

  return useMutation({
    mutationFn: authService.register,
    onSuccess: (data) => {
      const user = decodeToken(data.token);
      if (user) {
        setAuth(data.token, user);
        navigate("/");
      }
    },
  });
}

export function useLogout() {
  const logout = useAuthStore((state) => state.logout);
  const navigate = useNavigate();

  return () => {
    logout();
    navigate("/login");
  };
}
