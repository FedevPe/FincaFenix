import React, { type InputHTMLAttributes, type ReactNode, type SelectHTMLAttributes } from "react";
import { Checkbox } from "@/components/ui/checkbox";
import { cn } from "@/lib/utils";

interface FieldProps {
  label: string;
  htmlFor: string;
  error?: string;
  hint?: string;
  required?: boolean;
  children: ReactNode;
}

export function Field({ label, htmlFor, error, hint, required, children }: FieldProps) {
  const errorId = error ? `${htmlFor}-error` : undefined;
  const hintId = hint ? `${htmlFor}-hint` : undefined;

  return (
    <div className="flex flex-col gap-1.5">
      <label className="text-sm font-medium" htmlFor={htmlFor}>
        {label}
        {required ? <span className="text-destructive"> *</span> : null}
      </label>
      {React.isValidElement(children)
        ? React.cloneElement(children as React.ReactElement<any>, {
            "aria-describedby": cn(errorId, hintId),
            "aria-invalid": error ? true : undefined,
          })
        : children}
      {error ? (
        <span id={errorId} className="text-xs text-destructive" role="alert">
          {error}
        </span>
      ) : hint ? (
        <span id={hintId} className="text-xs text-muted-foreground">{hint}</span>
      ) : null}
    </div>
  );
}

interface TextInputProps extends InputHTMLAttributes<HTMLInputElement> {
  invalid?: boolean;
  "aria-describedby"?: string;
}

export function TextInput({ invalid, className, "aria-describedby": ariaDescribedBy, ...rest }: TextInputProps) {
  return (
    <input
      aria-invalid={invalid || undefined}
      aria-describedby={ariaDescribedBy}
      className={cn(
        "h-9 w-full rounded-md border bg-background px-3 py-1 text-sm shadow-xs transition-colors placeholder:text-muted-foreground focus-visible:ring-2 focus-visible:ring-ring focus-visible:outline-none disabled:cursor-not-allowed disabled:opacity-50",
        "aria-invalid:border-destructive aria-invalid:ring-destructive/20",
        className,
      )}
      {...rest}
    />
  );
}

interface SelectInputProps extends SelectHTMLAttributes<HTMLSelectElement> {
  invalid?: boolean;
  "aria-describedby"?: string;
}

export function SelectInput({
  invalid,
  className,
  children,
  "aria-describedby": ariaDescribedBy,
  ...rest
}: SelectInputProps) {
  return (
    <select
      className={cn(
        "h-9 w-full rounded-md border bg-background px-3 text-sm focus-visible:ring-2 focus-visible:ring-ring focus-visible:outline-none disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive",
        className,
      )}
      aria-invalid={invalid || undefined}
      aria-describedby={ariaDescribedBy}
      {...rest}
    >
      {children}
    </select>
  );
}

interface CheckboxRowProps {
  label: string;
  description?: string;
  checked: boolean;
  disabled?: boolean;
  onChange: (checked: boolean) => void;
}

export function CheckboxRow({
  label,
  description,
  checked,
  disabled,
  onChange,
}: CheckboxRowProps) {
  return (
    <label
      className={cn(
        "flex items-start gap-2",
        disabled && "cursor-not-allowed opacity-60",
      )}
    >
      <Checkbox
        checked={checked}
        disabled={disabled}
        onCheckedChange={(value) => onChange(value === true)}
        className="mt-0.5"
      />
      <span className="space-y-0.5">
        <span className="block text-sm">{label}</span>
        {description ? (
          <span className="block text-xs text-muted-foreground">{description}</span>
        ) : null}
      </span>
    </label>
  );
}
