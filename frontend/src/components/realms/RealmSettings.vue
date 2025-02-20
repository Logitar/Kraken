<script setup lang="ts">
import { TarCheckbox, type CheckboxOptions } from "logitar-vue3-ui";
import { ref } from "vue";

import PasswordSettingsEdit from "@/components/settings/PasswordSettingsEdit.vue";
import UniqueNameSettingsEdit from "@/components/settings/UniqueNameSettingsEdit.vue";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";

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
</script>

<template>
  <div>
    <!-- TODO(fpion): set/reset token secret -->
    <UniqueNameSettingsEdit v-model="uniqueNameSettings" />
    <PasswordSettingsEdit v-model="passwordSettings" />
    <!-- TODO(fpion): checkboxes (2) -->
  </div>
</template>
