<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import PasswordSettingsEdit from "@/components/settings/PasswordSettingsEdit.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UniqueNameSettingsEdit from "@/components/settings/UniqueNameSettingsEdit.vue";
import type { Configuration, ReplaceConfigurationPayload } from "@/types/configuration";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration, replaceConfiguration } from "@/api/configuration";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const passwordSettings = ref<PasswordSettings>({
  requiredLength: 8,
  requiredUniqueChars: 8,
  requireNonAlphanumeric: true,
  requireLowercase: true,
  requireUppercase: true,
  requireDigit: true,
  hashingStrategy: "PBKDF2",
});
const uniqueNameSettings = ref<UniqueNameSettings>({});

const hasChanges = computed<boolean>(() =>
  Boolean(
    configuration.value &&
      ((configuration.value.uniqueNameSettings.allowedCharacters ?? "") !== (uniqueNameSettings.value.allowedCharacters ?? "") ||
        configuration.value.passwordSettings.requiredLength !== passwordSettings.value.requiredLength ||
        configuration.value.passwordSettings.requiredUniqueChars !== passwordSettings.value.requiredUniqueChars ||
        configuration.value.passwordSettings.requireLowercase !== passwordSettings.value.requireLowercase ||
        configuration.value.passwordSettings.requireUppercase !== passwordSettings.value.requireUppercase ||
        configuration.value.passwordSettings.requireDigit !== passwordSettings.value.requireDigit ||
        configuration.value.passwordSettings.requireNonAlphanumeric !== passwordSettings.value.requireNonAlphanumeric ||
        configuration.value.passwordSettings.hashingStrategy !== passwordSettings.value.hashingStrategy),
  ),
);

function setModel(model: Configuration): void {
  configuration.value = model;
  uniqueNameSettings.value = { ...model.uniqueNameSettings };
  passwordSettings.value = { ...model.passwordSettings };
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: ReplaceConfigurationPayload = {
      uniqueNameSettings: uniqueNameSettings.value,
      passwordSettings: passwordSettings.value,
    };
    const updated: Configuration = await replaceConfiguration(payload, configuration.value?.version);
    setModel(updated);
    toasts.success("configuration.updated");
  } catch (e: unknown) {
    handleError(e);
  }
});

onMounted(async () => {
  try {
    const configuration: Configuration = await readConfiguration();
    setModel(configuration);
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="configuration">
      <h1>{{ t("configuration.title") }}</h1>
      <StatusDetail :aggregate="configuration" />
      <form @submit.prevent="onSubmit">
        <!-- TODO(fpion): set/reset token secret -->
        <UniqueNameSettingsEdit v-model="uniqueNameSettings" />
        <PasswordSettingsEdit v-model="passwordSettings" />
        <div class="mb-3">
          <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
        </div>
      </form>
    </template>
  </main>
</template>
