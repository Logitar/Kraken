import { describe, it, expect } from "vitest";

import type { ApiError, ProblemDetails } from "@/types/api";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { isError } from "../errors";

describe("isError", () => {
  it.concurrent("should return false when the actual code differs from the expected", () => {
    expect(isError({}, undefined, ErrorCodes.InvalidCredentials)).toBe(false);
    const problem: ProblemDetails = {};
    expect(isError({ data: problem }, undefined, ErrorCodes.InvalidCredentials)).toBe(false);
    problem.error = { code: ErrorCodes.UniqueNameAlreadyUsed, message: "The specified unique name is already used.", data: {} };
    expect(isError({ data: problem }, undefined, ErrorCodes.InvalidCredentials)).toBe(false);
  });

  it.concurrent("should return false when the actual status differs from the expected", () => {
    expect(isError({}, StatusCodes.NotFound)).toBe(false);
    const e: ApiError = { status: StatusCodes.BadRequest };
    expect(isError(e, StatusCodes.NotFound)).toBe(false);
  });

  it.concurrent("should return true when neither status nor code was specified", () => {
    expect(isError(undefined)).toBe(true);
  });

  it.concurrent("should return true when the error code matches", () => {
    const code: string = ErrorCodes.LocaleAlreadyUsed;
    const e: ApiError = {
      data: {
        error: {
          code,
          message: "The specified locale is already used.",
          data: {},
        },
      },
      status: StatusCodes.Conflict,
    };
    expect(isError(e, undefined, code)).toBe(true);
  });

  it.concurrent("should return true when the error code and status match", () => {
    const code: string = ErrorCodes.LocaleAlreadyUsed;
    const e: ApiError = {
      data: {
        error: {
          code,
          message: "The specified locale is already used.",
          data: {},
        },
      },
      status: StatusCodes.Conflict,
    };
    expect(isError(e, e.status, code)).toBe(true);
  });

  it.concurrent("should return true when the status matches", () => {
    const e: ApiError = { status: StatusCodes.NotFound };
    expect(isError(e, e.status)).toBe(true);
  });
});
