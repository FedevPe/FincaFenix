import { ChevronLeftIcon, ChevronRightIcon, MoreHorizontalIcon } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "./Button";
import {
  Pagination as PaginationRoot,
  PaginationContent,
  PaginationItem,
  PaginationEllipsis,
} from "@/components/ui/pagination";

interface PaginationProps {
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
}

function PaginationPageLink({
  page,
  isActive,
  onClick,
}: { page: number; isActive: boolean; onClick: (e: React.MouseEvent<HTMLButtonElement>) => void }) {
  return (
    <Button
      variant={isActive ? "primary" : "ghost"}
      size="sm"
      onClick={onClick}
      aria-current={isActive ? "page" : undefined}
      aria-label={`Ir a la página ${page}`}
      className="h-8 w-8 p-0 min-w-0"
    >
      {page}
    </Button>
  );
}

function PaginationPrevButton({
  onClick,
  disabled,
}: { onClick: (e: React.MouseEvent<HTMLButtonElement>) => void; disabled: boolean }) {
  return (
    <Button
      variant="ghost"
      size="sm"
      icon={ChevronLeftIcon}
      onClick={onClick}
      disabled={disabled}
      aria-label="Página anterior"
      className="h-8 w-8 p-0 min-w-0 opacity-50 disabled:opacity-30 disabled:cursor-not-allowed"
    />
  );
}

function PaginationNextButton({
  onClick,
  disabled,
}: { onClick: (e: React.MouseEvent<HTMLButtonElement>) => void; disabled: boolean }) {
  return (
    <Button
      variant="ghost"
      size="sm"
      icon={ChevronRightIcon}
      onClick={onClick}
      disabled={disabled}
      aria-label="Página siguiente"
      className="h-8 w-8 p-0 min-w-0 opacity-50 disabled:opacity-30 disabled:cursor-not-allowed"
    />
  );
}

function PaginationEllipsisItem() {
  return (
    <PaginationItem>
      <PaginationEllipsis
        className="flex h-8 w-8 items-center justify-center text-muted-foreground/60 hover:text-muted-foreground hover:bg-accent rounded-md transition-colors"
        title="Más páginas"
      >
        <MoreHorizontalIcon className="size-4" />
        <span className="sr-only">Más páginas</span>
      </PaginationEllipsis>
    </PaginationItem>
  );
}

export function Pagination({
  page,
  pageSize,
  totalCount,
  onPageChange,
}: PaginationProps) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const first = totalCount === 0 ? 0 : (page - 1) * pageSize + 1;
  const last = Math.min(page * pageSize, totalCount);

  const handlePageClick = (newPage: number) => (e: React.MouseEvent) => {
    e.preventDefault();
    onPageChange(newPage);
  };

  const pages = getPageNumbers(page, totalPages);

  return (
    <div className="flex w-full flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
      <div className="flex-1 text-sm text-muted-foreground shrink-0 sm:text-left text-center">
        {totalCount === 0 ? "Sin resultados" : `${first}-${last} de ${totalCount}`}
      </div>

      <PaginationRoot>
        <PaginationContent className="flex-1 flex justify-end sm:justify-end gap-1">
          <PaginationItem>
            <PaginationPrevButton
              onClick={handlePageClick(page - 1)}
              disabled={page <= 1}
            />
          </PaginationItem>

          {pages.map((p, i) =>
            p === "ellipsis" ? (
              <PaginationEllipsisItem key={`ellipsis-${i}`} />
            ) : (
              <PaginationItem key={p}>
                <PaginationPageLink
                  page={p}
                  isActive={p === page}
                  onClick={handlePageClick(p)}
                />
              </PaginationItem>
            )
          )}

          <PaginationItem>
            <PaginationNextButton
              onClick={handlePageClick(page + 1)}
              disabled={page >= totalPages}
            />
          </PaginationItem>
        </PaginationContent>
      </PaginationRoot>
    </div>
  );
}

function getPageNumbers(currentPage: number, totalPages: number): (number | "ellipsis")[] {
  if (totalPages <= 7) {
    return Array.from({ length: totalPages }, (_, i) => i + 1);
  }

  const pages: (number | "ellipsis")[] = [];
  const delta = 2;

  pages.push(1);

  if (currentPage - delta > 2) {
    pages.push("ellipsis");
  }

  const start = Math.max(2, currentPage - delta);
  const end = Math.min(totalPages - 1, currentPage + delta);

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  if (currentPage + delta < totalPages - 1) {
    pages.push("ellipsis");
  }

  pages.push(totalPages);

  return pages;
}