import { urlUtils } from "logitar-js";

import type { Configuration } from "@/types/configuration";
import { get } from ".";

function createUrlBuilder(): urlUtils.IUrlBuilder {
  return new urlUtils.UrlBuilder({ path: "/api/configuration" });
}

export async function readConfiguration(): Promise<Configuration> {
  const url: string = createUrlBuilder().buildRelative();
  return (await get<Configuration>(url)).data;
}
