import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { CustomAttribute } from "./custom";
import type { Role, RoleAction } from "./roles";
import type { SearchPayload, SortOption } from "./search";

export type ApiKey = Aggregate & {
  xApiKey?: string;
  name: string;
  description?: string;
  expiresOn?: string;
  authenticatedOn?: string;
  customAttributes: CustomAttribute[];
  roles: Role[];
};

export type ApiKeySort = "AuthenticatedOn" | "CreatedOn" | "ExpiresOn" | "Name" | "UpdatedOn";

export type ApiKeySortOption = SortOption & {
  field: ApiKeySort;
};

export type ApiKeyStatus = {
  isExpired: boolean;
  moment?: string;
};

export type CreateOrReplaceApiKeyPayload = {
  name: string;
  description?: string;
  expiresOn?: string;
  customAttributes: CustomAttribute[];
  roles: Role[];
};

export type SearchApiKeysPayload = SearchPayload & {
  hasAuthenticated?: boolean;
  roleId?: string;
  status?: ApiKeyStatus;
  sort: ApiKeySortOption[];
};

export type UpdateApiKeyPayload = {
  name?: string;
  description?: Change<string>;
  expiresOn?: string;
  customAttributes: CustomAttribute[];
  roles: RoleAction[];
};
