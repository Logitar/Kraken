import { urlUtils } from "logitar-js";

import type { CreateOrReplaceRolePayload, Role, SearchRolesPayload } from "@/types/roles";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/api/roles/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/api/roles" });
}

export async function createRole(payload: CreateOrReplaceRolePayload): Promise<Role> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceRolePayload, Role>(url, payload)).data;
}

export async function readRole(id: string): Promise<Role> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Role>(url)).data;
}

export async function replaceRole(id: string, payload: CreateOrReplaceRolePayload, version?: number): Promise<Role> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceRolePayload, Role>(url, payload)).data;
}

export async function searchRoles(payload: SearchRolesPayload): Promise<SearchResults<Role>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Role>>(url)).data;
}
