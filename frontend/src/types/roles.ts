import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { CustomAttribute } from "./custom";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

export type CollectionAction = "Add" | "Remove";

export type CreateOrReplaceRolePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  customAttributes: CustomAttribute[];
};

export type RoleAction = {
  role: string;
  action: CollectionAction;
};

export type Role = Aggregate & {
  uniqueName: string;
  displayName?: string;
  description?: string;
  customAttributes: CustomAttribute[];
  realm?: Realm;
};

export type RoleSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type RoleSortOption = SortOption & {
  field: RoleSort;
};

export type SearchRolesPayload = SearchPayload & {
  sort: RoleSortOption[];
};

export type UpdateRolePayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  customAttributes: CustomAttribute[];
};
