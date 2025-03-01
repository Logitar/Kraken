export type ConfirmedParams<T> = {
  value: T;
  label: string;
};

export type ValidationListeners = {
  blur: (e: unknown, shouldValidate?: boolean) => void;
  change: (e: unknown, shouldValidate?: boolean) => void;
  input: (e: unknown, shouldValidate?: boolean) => void;
};

export type ValidationRules = {
  allowed_characters?: string;
  confirmed?: string[];
  email?: boolean;
  identifier?: boolean;
  max_length?: number;
  max_value?: number;
  min_length?: number;
  min_value?: number;
  regex?: string;
  require_digit?: boolean;
  require_lowercase?: boolean;
  require_non_alphanumeric?: boolean;
  require_uppercase?: boolean;
  required?: boolean;
  slug?: boolean;
  unique_chars?: number;
  url?: boolean;
};
