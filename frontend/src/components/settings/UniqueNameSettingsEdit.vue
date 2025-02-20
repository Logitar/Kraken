<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { UniqueNameSettings } from "@/types/settings";

const { t } = useI18n();

const props = defineProps<{
  modelValue: UniqueNameSettings;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: UniqueNameSettings): void;
}>();

function onAllowedCharactersUpdate(allowedCharacters: string): void {
  const modelValue: UniqueNameSettings = { ...props.modelValue, allowedCharacters };
  emit("update:model-value", modelValue);
}
</script>

<template>
  <div>
    <h5>{{ t("settings.uniqueName.title") }}</h5>
    <AppInput
      class="mb-3"
      described-by="allowed-characters-help"
      floating
      id="allowed-characters"
      :label="t('settings.uniqueName.allowedCharacters.label')"
      max="255"
      :model-value="modelValue.allowedCharacters ?? ''"
      :placeholder="t('settings.uniqueName.allowedCharacters.label')"
      @update:model-value="onAllowedCharactersUpdate"
    >
      <template #after>
        <div id="allowed-characters-help" class="form-text">{{ t("settings.uniqueName.allowedCharacters.help") }}</div>
      </template>
    </AppInput>
  </div>
</template>
