import type { ApiError, ProblemDetails } from "@/types/api";

export function isError(e: unknown, status?: number, code?: string): boolean {
  if (!status && !code) {
    return true;
  }
  try {
    const apiError = e as ApiError;
    if (status && status !== apiError.status) {
      return false;
    }
    if (code) {
      const problemDetails = apiError.data as ProblemDetails;
      if (code !== problemDetails.error?.code) {
        return false;
      }
    }
    return true;
  } catch (e: unknown) {
    console.error(e);
    return false;
  }
}
