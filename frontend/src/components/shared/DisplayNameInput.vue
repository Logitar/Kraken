<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "./AppInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "display-name",
    label: "displayName",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="id"
    :label="t(label)"
    max="255"
    :model-value="modelValue"
    :placeholder="t(label)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
