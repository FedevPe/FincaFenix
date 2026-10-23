"use client";

import { useCallback, useEffect, useRef } from "react";

export interface UseAutoSaveOptions<T> {
  key: string;
  data: T;
  enabled?: boolean;
  debounceMs?: number;
  maxAgeMs?: number;
}

export interface UseAutoSaveReturn<T> {
  restore: () => T | null;
  clear: () => void;
}

interface StoredData<T> {
  data: T;
  timestamp: number;
}

export function useAutoSave<T>({
  key,
  data,
  enabled = true,
  debounceMs = 3000,
  maxAgeMs = 24 * 60 * 60 * 1000,
}: UseAutoSaveOptions<T>): UseAutoSaveReturn<T> {
  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const lastSavedRef = useRef<string>("");

  const save = useCallback(() => {
    if (!enabled) return;
    try {
      const serialized = JSON.stringify(data);
      if (serialized === lastSavedRef.current) return;
      const payload: StoredData<T> = {
        data,
        timestamp: Date.now(),
      };
      localStorage.setItem(key, JSON.stringify(payload));
      lastSavedRef.current = serialized;
    } catch {
      // Silently fail if localStorage is unavailable or quota exceeded
    }
  }, [key, data, enabled]);

  useEffect(() => {
    if (!enabled) return;
    if (timeoutRef.current) clearTimeout(timeoutRef.current);
    timeoutRef.current = setTimeout(save, debounceMs);
    return () => {
      if (timeoutRef.current) clearTimeout(timeoutRef.current);
    };
  }, [data, enabled, debounceMs, save]);

  const restore = useCallback((): T | null => {
    try {
      const stored = localStorage.getItem(key);
      if (!stored) return null;
      const parsed: StoredData<T> = JSON.parse(stored);
      if (!parsed.data || typeof parsed.timestamp !== "number") return null;
      if (Date.now() - parsed.timestamp > maxAgeMs) {
        localStorage.removeItem(key);
        return null;
      }
      lastSavedRef.current = JSON.stringify(parsed.data);
      return parsed.data;
    } catch {
      return null;
    }
  }, [key, maxAgeMs]);

  const clear = useCallback(() => {
    try {
      localStorage.removeItem(key);
      lastSavedRef.current = "";
    } catch {
      // Silently fail
    }
  }, [key]);

  return { restore, clear };
}