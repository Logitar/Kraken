<script setup lang="ts">
import { TarBadge, TarButton, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateApiKey from "@/components/keys/CreateApiKey.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { ApiKey, ApiKeySort, SearchApiKeysPayload } from "@/types/keys";
import { handleErrorKey } from "@/inject/App";
import { searchApiKeys } from "@/api/keys";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { d, rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const apiKeys = ref<ApiKey[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("apiKeys.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function isExpired(apiKey: ApiKey): boolean {
  const now: Date = new Date();
  return Boolean(apiKey.expiresOn && new Date(apiKey.expiresOn) <= now);
}

async function refresh(): Promise<void> {
  const payload: SearchApiKeysPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as ApiKeySort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchApiKeys(payload);
    if (now === timestamp.value) {
      apiKeys.value = results.items;
      total.value = results.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(apiKey: ApiKey) {
  toasts.success("apiKeys.created");
  if (apiKey.xApiKey) {
    sessionStorage.setItem("x-api-key", apiKey.xApiKey);
  }
  router.push({ name: "ApiKeyEdit", params: { id: apiKey.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ApiKeyList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                search: "",
                sort: "UpdatedOn",
                isDescending: "true",
                page: 1,
                count: 10,
              }
            : {
                page: 1,
                count: 10,
                ...query,
              },
        });
      } else {
        refresh();
      }
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <h1>{{ t("apiKeys.list") }}</h1>
    <div class="my-3">
      <TarButton
        class="me-1"
        :disabled="isLoading"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.refresh')"
        @click="refresh()"
      />
      <CreateApiKey class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <!-- TODO(fpion): hasAuthenticated, roleId, status -->
    <div class="row">
      <SearchInput class="col-lg-4" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-4"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-4" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="apiKeys.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("apiKeys.sort.options.Name") }}</th>
            <th scope="col">{{ t("apiKeys.sort.options.ExpiresOn") }}</th>
            <th scope="col">{{ t("apiKeys.sort.options.AuthenticatedOn") }}</th>
            <th scope="col">{{ t("apiKeys.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="apiKey in apiKeys" :key="apiKey.id">
            <td>
              <RouterLink :to="{ name: 'ApiKeyEdit', params: { id: apiKey.id } }"><font-awesome-icon icon="fas fa-edit" /> {{ apiKey.name }}</RouterLink>
            </td>
            <td>
              <TarBadge v-if="isExpired(apiKey)"><font-awesome-icon icon="fas fa-hourglass-end" /> {{ t("apiKeys.expired") }}</TarBadge>
              <template v-else-if="apiKey.expiresOn">{{ d(new Date(apiKey.expiresOn), "medium") }}</template>
              <template v-else>{{ "—" }}</template>
            </td>
            <td>{{ apiKey.authenticatedOn ? d(new Date(apiKey.authenticatedOn), "medium") : "—" }}</td>
            <td><StatusBlock :actor="apiKey.updatedBy" :date="apiKey.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("apiKeys.empty") }}</p>
  </main>
</template>
