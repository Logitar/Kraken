import type { Language } from "@/types/languages";
import type { Role } from "@/types/roles";

export function formatLanguage(language: Language): string {
  return `${language.locale.displayName} | ${language.locale.code}`;
}

export function formatRole(role: Role): string {
  return role.displayName ? `${role.displayName} (${role.uniqueName})` : role.uniqueName;
}
