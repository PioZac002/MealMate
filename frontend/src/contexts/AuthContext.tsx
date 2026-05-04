'use client';

import React, { createContext, useContext, useSyncExternalStore } from 'react';
import { authApi, type User } from '@/services/api';

interface AuthContextType {
  user: User | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<string | null>;
  register: (data: { email: string; password: string; firstName: string; lastName: string }) => Promise<string | null>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

const authListeners = new Set<() => void>();

function emitAuthChange() {
  authListeners.forEach((listener) => listener());
}

function subscribeToAuth(listener: () => void) {
  authListeners.add(listener);
  return () => authListeners.delete(listener);
}

function getStoredUser(): User | null {
  if (typeof window === 'undefined') {
    return null;
  }

  const storedUser = localStorage.getItem('user');
  const token = localStorage.getItem('accessToken');

  if (!storedUser || !token) {
    return null;
  }

  try {
    return JSON.parse(storedUser) as User;
  } catch {
    localStorage.removeItem('user');
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    return null;
  }
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const user = useSyncExternalStore(subscribeToAuth, getStoredUser, () => null);

  const login = async (email: string, password: string): Promise<string | null> => {
    const result = await authApi.login({ email, password });
    if (result.error) return result.error;
    if (result.data) {
      localStorage.setItem('accessToken', result.data.accessToken);
      localStorage.setItem('refreshToken', result.data.refreshToken);
      localStorage.setItem('user', JSON.stringify(result.data.user));
      emitAuthChange();
    }
    return null;
  };

  const register = async (data: {
    email: string; password: string; firstName: string; lastName: string
  }): Promise<string | null> => {
    const result = await authApi.register(data);
    if (result.error) return result.error;
    if (result.data) {
      localStorage.setItem('accessToken', result.data.accessToken);
      localStorage.setItem('refreshToken', result.data.refreshToken);
      localStorage.setItem('user', JSON.stringify(result.data.user));
      emitAuthChange();
    }
    return null;
  };

  const logout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    emitAuthChange();
  };

  return (
    <AuthContext.Provider value={{
      user,
      isLoading: false,
      isAuthenticated: !!user,
      login,
      register,
      logout,
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
