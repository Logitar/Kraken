<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { UsernameSettings } from "@/types/settings";
import type { ValidationRules } from "@/types/validation";

const { t } = useI18n();

const props = defineProps<{
  disabled?: boolean | string;
  modelValue?: string;
  required?: boolean | string;
  settings?: UsernameSettings;
}>();

const rules = computed<ValidationRules>(() => {
  const rules: ValidationRules = {};
  if (props.settings?.allowedCharacters) {
    rules.allowed_characters = props.settings.allowedCharacters;
  }
  return rules;
});

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <AppInput
    :disabled="disabled"
    floating
    id="username"
    :label="t('users.username')"
    max="255"
    :model-value="modelValue"
    :placeholder="t('users.username')"
    :required="required"
    :rules="rules"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
