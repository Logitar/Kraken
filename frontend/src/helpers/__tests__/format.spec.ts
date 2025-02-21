import { describe, it, expect } from "vitest";

import { formatLanguage } from "../format";
import type { Language } from "@/types/languages";
import type { Actor } from "@/types/actor";

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
