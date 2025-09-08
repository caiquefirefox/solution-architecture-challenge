import React, { createContext, useContext, useMemo, useState } from "react";
import { createApi } from "../../lib/api";
type TokenPair = {
  accessToken: string;
  refreshToken: string;
  expiresInSeconds: number;
};
type AuthCtx = {
  isAuthenticated: boolean;
  userName: string | null;
  login: (userNameOrEmail: string, password: string) => Promise<boolean>;
  register: (
    userName: string,
    email: string,
    password: string
  ) => Promise<boolean>;
  logout: (allDevices?: boolean) => Promise<void>;
  api: ReturnType<typeof createApi>;
};
const Ctx = createContext<AuthCtx | null>(null);
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [access, setAccess] = useState<string | null>(() =>
    localStorage.getItem("access")
  );
  const [refresh, setRefresh] = useState<string | null>(() =>
    localStorage.getItem("refresh")
  );
  const [userName, setUserName] = useState<string | null>(() =>
    localStorage.getItem("userName")
  );
  const api = useMemo(
    () =>
      createApi({
        getAccess: () => access,
        getRefresh: () => refresh,
        setTokens: (t: TokenPair) => {
          localStorage.setItem("access", t.accessToken);
          localStorage.setItem("refresh", t.refreshToken);
          setAccess(t.accessToken);
          setRefresh(t.refreshToken);
        },
        clear: () => {
          localStorage.removeItem("access");
          localStorage.removeItem("refresh");
          localStorage.removeItem("userName");
          setAccess(null);
          setRefresh(null);
          setUserName(null);
        },
      }),
    [access, refresh]
  );
  async function register(userName: string, email: string, password: string) {
    const res = await api.raw(
      "/api/v1/auth/register",
      { method: "POST", body: JSON.stringify({ userName, email, password }) },
      false
    );
    return res.ok;
  }
  async function login(userNameOrEmail: string, password: string) {
    const res = await api.raw(
      "/api/v1/auth/login",
      { method: "POST", body: JSON.stringify({ userNameOrEmail, password }) },
      false
    );
    if (!res.ok) return false;
    const tp = (await res.json()) as TokenPair;
    localStorage.setItem("access", tp.accessToken);
    setAccess(tp.accessToken);
    localStorage.setItem("refresh", tp.refreshToken);
    setRefresh(tp.refreshToken);
    localStorage.setItem("userName", userNameOrEmail);
    setUserName(userNameOrEmail);
    return true;
  }
  async function logout(allDevices = false) {
    if (access)
      await api.fetch("/api/v1/auth/logout", {
        method: "POST",
        body: JSON.stringify({ refreshToken: refresh, allDevices }),
      });
    localStorage.removeItem("access");
    localStorage.removeItem("refresh");
    localStorage.removeItem("userName");
    setAccess(null);
    setRefresh(null);
    setUserName(null);
  }
  const value: AuthCtx = {
    isAuthenticated: !!access,
    userName,
    login,
    register,
    logout,
    api,
  };
  return <Ctx.Provider value={value}>{children}</Ctx.Provider>;
}
export function useAuth() {
  const v = useContext(Ctx);
  if (!v) throw new Error("AuthProvider ausente");
  return v;
}
