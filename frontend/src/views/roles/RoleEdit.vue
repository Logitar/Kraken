<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import CustomAttributeList from "@/components/custom/CustomAttributeList.vue";
import RoleGeneral from "@/components/roles/RoleGeneral.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { Configuration } from "@/types/configuration";
import type { CustomAttribute } from "@/types/custom";
import type { Role, UpdateRolePayload } from "@/types/roles";
import { StatusCodes } from "@/enums/statusCodes";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readRole, updateRole } from "@/api/roles";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const role = ref<Role>();

function setMetadata(updated: Role): void {
  if (role.value) {
    role.value.version = updated.version;
    role.value.updatedBy = updated.updatedBy;
    role.value.updatedOn = updated.updatedOn;
  }
}

function onUpdate(updated: Role): void {
  if (role.value) {
    setMetadata(updated);
    role.value.uniqueName = updated.uniqueName;
    role.value.displayName = updated.displayName ?? "";
    role.value.description = updated.description ?? "";
  }
  toasts.success("roles.updated");
}

async function updateCustomAttributes(customAttributes: CustomAttribute[]): Promise<void> {
  if (role.value) {
    try {
      const payload: UpdateRolePayload = { customAttributes };
      const updated: Role = await updateRole(role.value.id, payload);
      setMetadata(updated);
      role.value.customAttributes = [...updated.customAttributes];
      toasts.success("roles.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      role.value = await readRole(id);
      configuration.value = await readConfiguration();
    }
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
    <template v-if="role && configuration">
      <h1>{{ role.displayName ?? role.uniqueName }}</h1>
      <StatusDetail :aggregate="role" />
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <RoleGeneral :role="role" :settings="configuration.uniqueNameSettings" @error="handleError" @updated="onUpdate" />
        </TarTab>
        <TarTab id="custom-attributes" :title="t('customAttributes.label')">
          <CustomAttributeList :custom-attributes="role.customAttributes" :save="updateCustomAttributes" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
