<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import PasswordSettingsEdit from "@/components/settings/PasswordSettingsEdit.vue";
import UniqueNameSettingsEdit from "@/components/settings/UniqueNameSettingsEdit.vue";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { Realm, UpdateRealmPayload } from "@/types/realms";
import { arePasswordEqual, areUniqueNameEqual } from "@/helpers/settings";
import { updateRealm } from "@/api/realms";

const { t } = useI18n();

const props = defineProps<{
  realm: Realm;
}>();

const passwordSettings = ref<PasswordSettings>({
  requiredLength: 8,
  requiredUniqueChars: 8,
  requireNonAlphanumeric: true,
  requireLowercase: true,
  requireUppercase: true,
  requireDigit: true,
  hashingStrategy: "PBKDF2",
});
const requireConfirmedAccount = ref<boolean>(true);
const requireUniqueEmail = ref<boolean>(true);
const uniqueNameSettings = ref<UniqueNameSettings>({});

const hasChanges = computed<boolean>(
  () =>
    !areUniqueNameEqual(props.realm.uniqueNameSettings, uniqueNameSettings.value) ||
    !arePasswordEqual(props.realm.passwordSettings, passwordSettings.value) ||
    props.realm.requireUniqueEmail !== requireUniqueEmail.value ||
    props.realm.requireConfirmedAccount !== requireConfirmedAccount.value,
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Realm): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: UpdateRealmPayload = {
      uniqueNameSettings: areUniqueNameEqual(props.realm.uniqueNameSettings, uniqueNameSettings.value) ? undefined : uniqueNameSettings.value,
      passwordSettings: arePasswordEqual(props.realm.passwordSettings, passwordSettings.value) ? undefined : passwordSettings.value,
      requireUniqueEmail: props.realm.requireUniqueEmail !== requireUniqueEmail.value ? requireUniqueEmail.value : undefined,
      requireConfirmedAccount: props.realm.requireConfirmedAccount !== requireConfirmedAccount.value ? requireConfirmedAccount.value : undefined,
      customAttributes: [],
    };
    const realm: Realm = await updateRealm(props.realm.id, payload);
    emit("updated", realm);
  } catch (e: unknown) {
    emit("error", e);
  }
});

watch(
  () => props.realm,
  (realm) => {
    uniqueNameSettings.value = { ...realm.uniqueNameSettings };
    passwordSettings.value = { ...realm.passwordSettings };
    requireUniqueEmail.value = realm.requireUniqueEmail;
    requireConfirmedAccount.value = realm.requireConfirmedAccount;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="onSubmit">
    <!-- TODO(fpion): set/reset token secret -->
    <UniqueNameSettingsEdit v-model="uniqueNameSettings" />
    <PasswordSettingsEdit v-model="passwordSettings" />
    <h5>{{ t("settings.users.title") }}</h5>
    <div class="mb-3">
      <TarCheckbox id="require-unique-email" :label="t('settings.users.requireUniqueEmail.label')" v-model="requireUniqueEmail" />
      <div class="form-text">
        <!-- TODO(fpion): help -->
      </div>
    </div>
    <div class="mb-3">
      <TarCheckbox id="require-confirmed-account" :label="t('settings.users.requireConfirmedAccount.label')" v-model="requireConfirmedAccount" />
      <div class="form-text">
        <!-- TODO(fpion): help -->
      </div>
    </div>
    <div class="mb-3">
      <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
    </div>
  </form>
</template>
