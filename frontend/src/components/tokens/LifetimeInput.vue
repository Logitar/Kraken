<script setup lang="ts">
import { useI18n } from "vue-i18n";

import NumberInput from "@/components/shared/NumberInput.vue";

const { t } = useI18n();

defineProps<{
  modelValue?: number;
}>();

defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();
</script>

<template>
  <NumberInput
    floating
    id="lifetime"
    :label="t('tokens.lifetime.label')"
    min="0"
    :model-value="modelValue"
    :placeholder="t('tokens.lifetime.label')"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #append>
      <span class="input-group-text">{{ t("tokens.lifetime.unit", modelValue ?? 0) }}</span>
    </template>
  </NumberInput>
</template>
