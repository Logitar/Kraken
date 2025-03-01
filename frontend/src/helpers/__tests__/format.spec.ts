import { describe, it, expect } from "vitest";

import type { Actor } from "@/types/actor";
import type { Language } from "@/types/languages";
import type { Role } from "@/types/roles";
import { formatLanguage, formatRole } from "../format";

const actor: Actor = {
  id: "user_id",
  type: "User",
  isDeleted: false,
  displayName: "Francis Pion",
};
const now: string = new Date().toISOString();

describe("formatLanguage", () => {
  it.concurrent("should format a language correctly", () => {
    const language: Language = {
      id: "language_id",
      version: 1,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      isDefault: true,
      locale: {
        id: 9,
        code: "en",
        displayName: "English",
        englishName: "English",
        nativeName: "English",
      },
    };
    expect(formatLanguage(language)).toBe("English | en");
  });
});

describe("formatRole", () => {
  it.concurrent("should format a role without display name", () => {
    const role: Role = {
      id: "role_id",
      version: 1,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      uniqueName: "admin",
      customAttributes: [],
    };
    expect(formatRole(role)).toBe(role.uniqueName);
  });

  it.concurrent("should format a role with a display name", () => {
    const role: Role = {
      id: "role_id",
      version: 1,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      uniqueName: "admin",
      displayName: "Administrator",
      customAttributes: [],
    };
    expect(formatRole(role)).toBe(`${role.displayName} (${role.uniqueName})`);
  });
});
