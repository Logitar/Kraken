<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import CustomAttributeList from "@/components/custom/CustomAttributeList.vue";
import RealmGeneral from "@/components/realms/RealmGeneral.vue";
import RealmSettings from "@/components/realms/RealmSettings.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { Configuration } from "@/types/configuration";
import type { CustomAttribute } from "@/types/custom";
import type { Realm, UpdateRealmPayload } from "@/types/realms";
import { StatusCodes } from "@/enums/statusCodes";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readRealm, updateRealm } from "@/api/realms";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const realm = ref<Realm>();

function setMetadata(updated: Realm): void {
  if (realm.value) {
    realm.value.version = updated.version;
    realm.value.updatedBy = updated.updatedBy;
    realm.value.updatedOn = updated.updatedOn;
  }
}

function onGeneralUpdate(updated: Realm): void {
  if (realm.value) {
    setMetadata(updated);
    realm.value.uniqueSlug = updated.uniqueSlug;
    realm.value.displayName = updated.displayName ?? "";
    realm.value.description = updated.description ?? "";
    realm.value.url = updated.url ?? "";
  }
  toasts.success("realms.updated");
}
function onSettingsUpdate(updated: Realm): void {
  if (realm.value) {
    setMetadata(updated);
    realm.value.uniqueNameSettings = { ...updated.uniqueNameSettings };
    realm.value.passwordSettings = { ...updated.passwordSettings };
    realm.value.requireUniqueEmail = updated.requireUniqueEmail;
    realm.value.requireConfirmedAccount = updated.requireConfirmedAccount;
  }
  toasts.success("realms.updated");
}

async function updateCustomAttributes(customAttributes: CustomAttribute[]): Promise<void> {
  if (realm.value) {
    try {
      const payload: UpdateRealmPayload = { customAttributes };
      const updated: Realm = await updateRealm(realm.value.id, payload);
      setMetadata(updated);
      realm.value.customAttributes = [...updated.customAttributes];
      toasts.success("realms.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      realm.value = await readRealm(id);
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
    <template v-if="realm && configuration">
      <h1>{{ realm.displayName ?? realm.uniqueSlug }}</h1>
      <StatusDetail :aggregate="realm" />
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <RealmGeneral :realm="realm" :settings="configuration.uniqueNameSettings" @error="handleError" @updated="onGeneralUpdate" />
        </TarTab>
        <TarTab id="settings" :title="t('settings.title')">
          <RealmSettings :realm="realm" @error="handleError" @updated="onSettingsUpdate" />
        </TarTab>
        <TarTab id="custom-attributes" :title="t('customAttributes.label')">
          <CustomAttributeList :custom-attributes="realm.customAttributes" :save="updateCustomAttributes" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
