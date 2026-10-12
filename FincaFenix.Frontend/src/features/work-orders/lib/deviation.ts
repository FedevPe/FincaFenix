import { formatNumber, toNumber } from "@/lib/utils";

export function deviationPercent(value: unknown, baseline: unknown): number | null {
  const base = toNumber(baseline);
  if (base <= 0) return null;
  return ((toNumber(value) - base) / base) * 100;
}

export function isOverrun(value: unknown, baseline: unknown): boolean {
  const base = toNumber(baseline);
  return base >= 0 && toNumber(value) > base;
}

export function overrunTone(value: unknown, baseline: unknown): string {
  return isOverrun(value, baseline) ? "text-destructive" : "text-success";
}

export function efficiencyTone(percent: number | null | undefined): string {
  if (percent == null || !Number.isFinite(percent)) return "";
  return percent > 100 ? "text-destructive" : "text-success";
}

export function formatDeviation(percent: number | null | undefined): string {
  if (percent == null || !Number.isFinite(percent)) return "-";
  const sign = percent > 0 ? "+" : "";
  return `${sign}${formatNumber(percent, { maximumFractionDigits: 1 })}%`;
}
