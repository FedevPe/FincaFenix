"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useSearchParams } from "next/navigation";
import { Settings as SettingsIcon } from "lucide-react";
import { PageHeader } from "@/components/common/PageHeader";
import { Tabs } from "@/components/common/Tabs";
import type { TabItem } from "@/components/common/Tabs";
import { EmptyState } from "@/components/common/Feedback";
import { useAuth } from "@/app/providers/AuthProvider";
import { hasPolicy } from "@/lib/utils";
import { SETTINGS_TABS, type SettingsTab } from "../constants";
import { FincasTab } from "../tabs/FincasTab";
import { MaterialesTab } from "../tabs/MaterialesTab";
import { TareasTab } from "../tabs/TareasTab";
import { EmpleadosTab } from "../tabs/EmpleadosTab";
import { CategoriasTab } from "../tabs/CategoriasTab";
import { UnidadesTab } from "../tabs/UnidadesTab";
import { FrutasTab } from "../tabs/FrutasTab";
import { VariedadesTab } from "../tabs/VariedadesTab";
import { UsuariosTab } from "../tabs/UsuariosTab";

export default function Settings() {
  const searchParams = useSearchParams();
  const { user } = useAuth();

  const allowedTabs = useMemo(
    () => SETTINGS_TABS.filter((tab) => hasPolicy(user?.policies, tab.policy)),
    [user?.policies],
  );

  const isAllowed = useCallback(
    (value: string | null): value is SettingsTab =>
      allowedTabs.some((tab) => tab.id === value),
    [allowedTabs],
  );

  const tabValue = searchParams.get("tab");
  const [activeTab, setActiveTab] = useState<SettingsTab>(
    isAllowed(tabValue) ? tabValue : (allowedTabs[0]?.id ?? "fincas"),
  );

  useEffect(() => {
    if (allowedTabs.length === 0) return;
    if (!allowedTabs.some((tab) => tab.id === activeTab)) {
      setActiveTab(allowedTabs[0].id);
    }
  }, [allowedTabs, activeTab]);

  useEffect(() => {
    if (typeof window === "undefined" || allowedTabs.length === 0) return;
    const url =
      activeTab === allowedTabs[0].id
        ? "/settings"
        : `/settings?tab=${activeTab}`;
    window.history.replaceState(null, "", url);
  }, [activeTab, allowedTabs]);

  const tabs: TabItem<SettingsTab>[] = allowedTabs;

  return (
    <div className="space-y-6">
      <PageHeader
        title="Configuración"
        subtitle="Administración de los datos maestros del sistema."
      />

      {allowedTabs.length === 0 ? (
        <EmptyState
          title="Acceso restringido"
          description="No tenés permisos para ver la configuración del sistema."
          icon={SettingsIcon}
        />
      ) : (
        <>
          <Tabs<SettingsTab>
            tabs={tabs}
            active={activeTab}
            onChange={setActiveTab}
          />
          {activeTab === "usuarios" ? <UsuariosTab /> : null}
          {activeTab === "fincas" ? <FincasTab /> : null}
          {activeTab === "materiales" ? <MaterialesTab /> : null}
          {activeTab === "empleados" ? <EmpleadosTab /> : null}
          {activeTab === "tareas" ? <TareasTab /> : null}
          {activeTab === "unidades" ? <UnidadesTab /> : null}
          {activeTab === "categorias" ? <CategoriasTab /> : null}
          {activeTab === "frutas" ? <FrutasTab /> : null}
          {activeTab === "variedades" ? <VariedadesTab /> : null}
        </>
      )}
    </div>
  );
}
