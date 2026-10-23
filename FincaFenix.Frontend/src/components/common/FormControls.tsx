"use client";

import * as React from "react";
import type {
  InputHTMLAttributes,
  ReactNode,
  SelectHTMLAttributes,
  TextareaHTMLAttributes,
} from "react";

import { Checkbox } from "@/components/ui/checkbox";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
  useFormField,
} from "@/components/ui/form";
import { InputGroup, InputGroupAddon } from "@/components/ui/input-group";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Textarea as TextareaPrimitive } from "@/components/ui/textarea";
import { cn } from "@/lib/utils";

/* -------------------------------------------------------------------------- */
/* Field                                                                      */
/* -------------------------------------------------------------------------- */

interface FieldProps {
  label: string;
  htmlFor: string;
  error?: string;
  hint?: string;
  required?: boolean;
  children: ReactNode;
}

export function Field({ label, htmlFor, error, hint, required, children }: FieldProps) {
  const hasError = Boolean(error);
  const errorId = hasError ? `${htmlFor}-error` : undefined;
  const hintId = hint && !hasError ? `${htmlFor}-hint` : undefined;

  const childProps = React.isValidElement(children)
    ? (children.props as Record<string, unknown>)
    : undefined;
  const childDescribedBy = childProps?.["aria-describedby"] as string | undefined;
  const childAriaInvalid = childProps?.["aria-invalid"] as
    | boolean
    | "true"
    | "false"
    | undefined;

  const describedBy =
    [childDescribedBy, errorId, hintId].filter(Boolean).join(" ") || undefined;
  const ariaInvalid = hasError || childAriaInvalid || undefined;

  return (
    <div className="flex flex-col gap-1.5">
      <Label htmlFor={htmlFor} className="text-sm font-medium">
        {label}
        {required ? (
          <span className="text-destructive" aria-hidden="true">
            {" "}*
          </span>
        ) : null}
      </Label>

      {React.isValidElement(children)
        ? React.cloneElement(children as React.ReactElement<any>, {
            "aria-describedby": describedBy,
            "aria-invalid": ariaInvalid,
          })
        : children}

      {hasError ? (
        <p id={errorId} className="text-xs text-destructive" role="alert">
          {error}
        </p>
      ) : hint ? (
        <p id={hintId} className="text-xs text-muted-foreground">
          {hint}
        </p>
      ) : null}
    </div>
  );
}

/* -------------------------------------------------------------------------- */
/* TextInput                                                                  */
/* -------------------------------------------------------------------------- */

interface TextInputProps extends InputHTMLAttributes<HTMLInputElement> {
  invalid?: boolean;
}

export const TextInput = React.forwardRef<HTMLInputElement, TextInputProps>(
  function TextInput({ invalid, className, ...props }, ref) {
    const { "aria-invalid": ariaInvalid, ...rest } = props;

    return (
      <Input
        ref={ref}
        className={className}
        aria-invalid={invalid || ariaInvalid || undefined}
        {...rest}
      />
    );
  },
);
TextInput.displayName = "TextInput";

/* -------------------------------------------------------------------------- */
/* Textarea                                                                   */
/* -------------------------------------------------------------------------- */

interface TextareaProps extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  invalid?: boolean;
}

export const Textarea = React.forwardRef<HTMLTextAreaElement, TextareaProps>(
  function Textarea({ invalid, className, ...props }, ref) {
    const { "aria-invalid": ariaInvalid, ...rest } = props;

    return (
      <TextareaPrimitive
        ref={ref}
        className={className}
        aria-invalid={invalid || ariaInvalid || undefined}
        {...rest}
      />
    );
  },
);
Textarea.displayName = "Textarea";

/* -------------------------------------------------------------------------- */
/* SelectInput                                                                */
/* -------------------------------------------------------------------------- */

type ParsedSelectOption = {
  value: string;
  label: ReactNode;
  textLabel: string;
  disabled?: boolean;
  groupLabel?: ReactNode;
};

function nodeToText(node: ReactNode): string {
  if (node === null || node === undefined || typeof node === "boolean") return "";
  if (typeof node === "string" || typeof node === "number") return String(node);
  if (Array.isArray(node)) return node.map(nodeToText).join("");
  if (React.isValidElement<{ children?: ReactNode }>(node)) {
    return nodeToText(node.props.children);
  }
  return "";
}

function parseSelectChildren(children: ReactNode): {
  options: ParsedSelectOption[];
  placeholder?: string;
} {
  const options: ParsedSelectOption[] = [];

  const addOption = (element: React.ReactElement<any>, groupLabel?: ReactNode) => {
    const props = element.props;
    const label = props.children as ReactNode;

    options.push({
      value: String(props.value ?? ""),
      label,
      textLabel: nodeToText(label),
      disabled: props.disabled,
      groupLabel,
    });
  };

  const visit = (nodes: ReactNode, groupLabel?: ReactNode) => {
    React.Children.forEach(nodes, (child) => {
      if (!React.isValidElement(child)) return;
      const element = child as React.ReactElement<any>;

      if (element.type === "optgroup") {
        visit(element.props.children, element.props.label);
        return;
      }

      if (element.type === "option") {
        addOption(element, groupLabel);
        return;
      }

      if (element.type === React.Fragment) {
        visit(element.props.children, groupLabel);
      }
    });
  };

  visit(children);

  const placeholderOption = options.find((option) => option.value === "");

  return {
    options: options.filter((option) => option.value !== ""),
    placeholder: placeholderOption?.textLabel || undefined,
  };
}

function toSelectValue(
  value: string | number | readonly string[] | null | undefined,
): string {
  if (value === null || value === undefined) return "";
  if (Array.isArray(value)) return value.length > 0 ? String(value[0]) : "";
  return String(value);
}

interface SelectInputProps
  extends Omit<SelectHTMLAttributes<HTMLSelectElement>, "onChange"> {
  invalid?: boolean;
  placeholder?: string;
  onChange?: (event: React.ChangeEvent<HTMLSelectElement>) => void;
  ref?: React.Ref<any>;
}

export function SelectInput({
  id,
  className,
  children,
  invalid,
  value,
  defaultValue,
  placeholder,
  disabled,
  required,
  name,
  onChange,
  onBlur,
  onFocus,
  title,
  autoFocus,
  "aria-describedby": ariaDescribedBy,
  "aria-invalid": ariaInvalidProp,
  "aria-label": ariaLabel,
  "aria-labelledby": ariaLabelledBy,
  ref,
}: SelectInputProps) {
  const isControlled = value !== undefined;
  const [internalValue, setInternalValue] = React.useState<string>(() =>
    toSelectValue(defaultValue),
  );
  const triggerRef = React.useRef<HTMLButtonElement>(null);

  const selectedValue = isControlled ? toSelectValue(value) : internalValue;
  const parsed = parseSelectChildren(children);
  const resolvedPlaceholder =
    placeholder ?? parsed.placeholder ?? "Seleccionar…";

  /* Preserve the native `select` ref contract for react-hook-form. RHF writes
   * `.value` on the referenced element when the form is reset, so we read it
   * back after render to keep the shadcn Select in sync. */
  const setTriggerRef = React.useCallback(
    (node: HTMLButtonElement | null) => {
      triggerRef.current = node;
      if (typeof ref === "function") {
        ref(node);
      } else if (ref && typeof ref === "object") {
        (ref as { current: HTMLButtonElement | null }).current = node;
      }
    },
    [ref],
  );

  React.useEffect(() => {
    if (isControlled) return;
    const node = triggerRef.current;
    if (node && node.value !== internalValue) {
      setInternalValue(node.value);
    }
  });

  const handleValueChange = (nextValue: string) => {
    if (!isControlled) {
      setInternalValue(nextValue);
    }
    if (triggerRef.current) {
      triggerRef.current.value = nextValue;
    }

    onChange?.({
      type: "change",
      target: { value: nextValue, name },
      currentTarget: { value: nextValue, name },
    } as unknown as React.ChangeEvent<HTMLSelectElement>);
  };

  const groups: { label?: ReactNode; options: ParsedSelectOption[] }[] = [];
  for (const option of parsed.options) {
    const last = groups[groups.length - 1];
    if (last && last.label === option.groupLabel) {
      last.options.push(option);
    } else {
      groups.push({ label: option.groupLabel, options: [option] });
    }
  }

  return (
    <Select
      value={selectedValue}
      onValueChange={handleValueChange}
      disabled={disabled}
      required={required}
    >
      <SelectTrigger
        ref={setTriggerRef}
        id={id}
        className={cn("w-full", className)}
        disabled={disabled}
        aria-invalid={invalid || ariaInvalidProp || undefined}
        aria-describedby={ariaDescribedBy}
        aria-label={ariaLabel}
        aria-labelledby={ariaLabelledBy}
        title={title}
        autoFocus={autoFocus}
        onBlur={onBlur as unknown as React.FocusEventHandler<HTMLButtonElement>}
        onFocus={onFocus as unknown as React.FocusEventHandler<HTMLButtonElement>}
      >
        <SelectValue placeholder={resolvedPlaceholder} />
      </SelectTrigger>

      <SelectContent>
        {groups.map((group, index) => (
          <SelectGroup key={index}>
            {group.label !== undefined ? (
              <SelectLabel>{group.label}</SelectLabel>
            ) : null}
            {group.options.map((option) => (
              <SelectItem
                key={option.value}
                value={option.value}
                disabled={option.disabled}
              >
                {option.label}
              </SelectItem>
            ))}
          </SelectGroup>
        ))}
      </SelectContent>
    </Select>
  );
}

/* -------------------------------------------------------------------------- */
/* CheckboxRow                                                                */
/* -------------------------------------------------------------------------- */

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
  const descriptionId = React.useId();

  return (
    <Label
      className={cn(
        "flex items-start gap-2 font-normal",
        disabled && "cursor-not-allowed opacity-60",
      )}
    >
      <Checkbox
        checked={checked}
        disabled={disabled}
        onCheckedChange={(next) => onChange(next === true)}
        aria-describedby={description ? descriptionId : undefined}
        className="mt-0.5"
      />
      <span className="flex flex-col gap-0.5">
        <span className="text-sm leading-snug">{label}</span>
        {description ? (
          <span
            id={descriptionId}
            className="text-xs text-muted-foreground"
          >
            {description}
          </span>
        ) : null}
      </span>
    </Label>
  );
}

/* -------------------------------------------------------------------------- */
/* Re-exports: shadcn/ui building blocks                                      */
/* -------------------------------------------------------------------------- */

export {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormControl,
  FormDescription,
  FormMessage,
  useFormField,
  InputGroup,
  InputGroupAddon,
  SelectItem,
};