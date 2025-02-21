import type { Language } from "@/types/languages";

export function formatLanguage(language: Language): string {
  return `${language.locale.displayName} | ${language.locale.code}`;
}
