"use client";

import type { LucideIcon } from "lucide-react";
import * as React from "react";
import { cn } from "@/lib/utils";
import { Tabs as TabsPrimitive, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";

export interface TabItem<T extends string> {
  id: T;
  label: string;
  icon?: LucideIcon;
  disabled?: boolean;
  content?: React.ReactNode;
}

interface TabsProps<T extends string> {
  tabs: TabItem<T>[];
  active: T;
  onChange: (id: T) => void;
}

export function Tabs<T extends string>({ tabs, active, onChange }: TabsProps<T>) {
  return (
    <TabsPrimitive value={active} onValueChange={onChange}>
      <TabsList className="bg-muted/40 p-1 rounded-lg">
        {tabs.map((tab) => {
          const Icon = tab.icon;
          return (
            <TabsTrigger
              key={tab.id}
              value={tab.id}
              disabled={tab.disabled}
              className="data-[state=active]:bg-background data-[state=active]:shadow-sm"
            >
              {Icon && <Icon className="size-4" aria-hidden="true" />}
              {tab.label}
            </TabsTrigger>
          );
        })}
      </TabsList>
      {tabs.map((tab) => (
        <TabsContent key={tab.id} value={tab.id}>
          {tab.content}
        </TabsContent>
      ))}
    </TabsPrimitive>
  );
}