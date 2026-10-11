import { Suspense } from "react";
import Inventory from "@/features/inventory/pages/Inventory";
import { LoadingState } from "@/components/common/Feedback";

export default function Page() {
  return (
    <Suspense fallback={<LoadingState />}>
      <Inventory />
    </Suspense>
  );
}
