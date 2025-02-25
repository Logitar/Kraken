<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ApiKeyGeneral from "@/components/keys/ApiKeyGeneral.vue";
import CustomAttributeList from "@/components/custom/CustomAttributeList.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import XApiKey from "@/components/keys/XApiKey.vue";
import type { ApiError } from "@/types/api";
import type { ApiKey, UpdateApiKeyPayload } from "@/types/keys";
import type { CustomAttribute } from "@/types/custom";
import { StatusCodes } from "@/enums/statusCodes";
import { handleErrorKey } from "@/inject/App";
import { readApiKey, updateApiKey } from "@/api/keys";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { d, t } = useI18n();

const apiKey = ref<ApiKey>();
const xApiKey = ref<string>();

function setMetadata(updated: ApiKey): void {
  if (apiKey.value) {
    apiKey.value.version = updated.version;
    apiKey.value.updatedBy = updated.updatedBy;
    apiKey.value.updatedOn = updated.updatedOn;
  }
}

function onUpdate(updated: ApiKey): void {
  if (apiKey.value) {
    setMetadata(updated);
    apiKey.value.name = updated.name;
    apiKey.value.description = updated.description ?? "";
    apiKey.value.expiresOn = updated.expiresOn ?? "";
  }
  toasts.success("apiKeys.updated");
}

async function updateCustomAttributes(customAttributes: CustomAttribute[]): Promise<void> {
  if (apiKey.value) {
    try {
      const payload: UpdateApiKeyPayload = { customAttributes, roles: [] };
      const updated: ApiKey = await updateApiKey(apiKey.value.id, payload);
      setMetadata(updated);
      apiKey.value.customAttributes = [...updated.customAttributes];
      toasts.success("apiKeys.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      apiKey.value = await readApiKey(id);
    }
    xApiKey.value = sessionStorage.getItem("x-api-key") ?? undefined;
    sessionStorage.removeItem("x-api-key");
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === StatusCodes.NotFound) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="apiKey">
      <h1>{{ apiKey.name }}</h1>
      <XApiKey v-if="xApiKey" :value="xApiKey" />
      <StatusDetail :aggregate="apiKey" />
      <p v-if="apiKey.authenticatedOn">{{ t("apiKeys.authenticatedOnFormat", { date: d(new Date(apiKey.authenticatedOn), "medium") }) }}</p>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <ApiKeyGeneral :apiKey="apiKey" @error="handleError" @updated="onUpdate" />
        </TarTab>
        <TarTab id="custom-attributes" :title="t('customAttributes.label')">
          <CustomAttributeList :custom-attributes="apiKey.customAttributes" :save="updateCustomAttributes" />
        </TarTab>
        <!-- TODO(fpion): Roles -->
      </TarTabs>
    </template>
  </main>
</template>
