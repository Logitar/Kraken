import type { Aggregate } from "./aggregate";
import type { PasswordSettings, UniqueNameSettings } from "./settings";

export type Configuration = Aggregate & {
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
};

export type ReplaceConfigurationPayload = {
  secret?: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
};
