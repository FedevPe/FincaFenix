import { Suspense } from "react";
import WorkOrderList from "@/features/work-orders/pages/WorkOrderList";
import { LoadingState } from "@/components/common/Feedback";

export default function Page() {
  return (
    <Suspense fallback={<LoadingState />}>
      <WorkOrderList />
    </Suspense>
  );
}
