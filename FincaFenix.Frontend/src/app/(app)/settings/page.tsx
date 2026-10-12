import { Suspense } from "react";
import { LoadingState } from "@/components/common/Feedback";
import Settings from "@/features/settings/pages/Settings";

export default function SettingsPage() {
  return (
    <Suspense fallback={<LoadingState />}>
      <Settings />
    </Suspense>
  );
}
