<script setup lang="ts">
import type { InputOptions } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";

const { t } = useI18n();

const props = withDefaults(defineProps<InputOptions>(), {
  floating: true,
  id: "type",
  label: "tokens.type",
  placeholder: "tokens.type",
});

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <AppInput v-bind="props" :label="t(label)" :placeholder="t(placeholder)" @update:model-value="$emit('update:model-value', $event)" />
</template>
