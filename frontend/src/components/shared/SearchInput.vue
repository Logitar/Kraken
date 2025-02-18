<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "./AppInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    label?: string;
    modelValue?: string;
  }>(),
  {
    label: "search",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="search"
    :label="t(label)"
    :model-value="modelValue"
    :placeholder="t(label)"
    type="search"
    validation="server"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
