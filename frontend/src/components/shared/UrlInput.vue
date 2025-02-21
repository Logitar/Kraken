<script setup lang="ts">
import { TarButton, type InputOptions } from "logitar-vue3-ui";

import AppInput from "./AppInput.vue";

const props = withDefaults(
  defineProps<
    InputOptions & {
      goText?: string;
    }
  >(),
  {
    max: 2048,
    type: "url",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function go(): void {
  window.open(props.modelValue, "_blank");
}
</script>

<template>
  <AppInput v-bind="props" :rules="{ url: true }" @update:model-value="$emit('update:model-value', $event)">
    <template #before>
      <slot name="before"></slot>
    </template>
    <template #prepend>
      <slot name="prepend"></slot>
    </template>
    <template #label-override>
      <slot name="label-override"></slot>
    </template>
    <template #label-required>
      <slot name="label-required"></slot>
    </template>
    <template #append>
      <TarButton :disabled="!modelValue" icon="fas fa-arrow-up-right-from-square" :text="goText" variant="info" @click="go" />
      <slot name="append"></slot>
    </template>
    <template #after>
      <slot name="after"></slot>
    </template>
  </AppInput>
</template>
