<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import AppCheckbox from "./AppCheckbox.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    descending?: boolean | string;
    id?: string;
    modelValue?: string;
    options?: SelectOption[];
  }>(),
  {
    id: "sort",
  },
);

defineEmits<{
  (e: "descending", value: boolean): void;
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <TarSelect
    floating
    :id="id"
    :label="t('sort.select.label')"
    :model-value="modelValue"
    :options="options"
    :placeholder="t('sort.select.placeholder')"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <AppCheckbox :id="`${id}_desc`" :label="t('sort.isDescending')" :model-value="descending" tight @update:model-value="$emit('descending', $event)" />
    </template>
  </TarSelect>
</template>
