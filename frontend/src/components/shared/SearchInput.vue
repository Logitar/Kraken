<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

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
  <TarInput
    floating
    id="search"
    :label="t(label)"
    :model-value="modelValue"
    :placeholder="t(label)"
    type="search"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
