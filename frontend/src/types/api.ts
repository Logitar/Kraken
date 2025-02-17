export type ApiError = {
  data?: unknown;
  status: number;
};

export type ApiResult<T> = {
  data: T;
  status: number;
};

export type ApiVersion = {
  title: string;
  version: string;
};

export type Error = {
  code: string;
  message: string;
  data: unknown;
};

export type ProblemDetails = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  traceId?: string;
  error?: Error;
  errors?: ValidationFailure[];
};

export type Severity = "Error" | "Warning" | "Info";

export type ValidationFailure = {
  attemptedValue?: unknown;
  customState?: unknown;
  errorCode?: string;
  errorMessage?: string;
  propertyName?: string;
  severity: Severity;
};
