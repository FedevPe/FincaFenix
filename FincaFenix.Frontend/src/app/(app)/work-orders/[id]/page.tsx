import { Suspense } from "react";
import WorkOrderDetail from "@/features/work-orders/pages/WorkOrderDetail";
import { LoadingState } from "@/components/common/Feedback";

export default function Page() {
  return (
    <Suspense fallback={<LoadingState />}>
      <WorkOrderDetail />
    </Suspense>
  );
}
