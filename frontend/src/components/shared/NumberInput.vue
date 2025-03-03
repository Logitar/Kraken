<script setup lang="ts">
import type { InputSize, InputStatus } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";

import AppInput from "./AppInput.vue";
import type { ValidationRules } from "@/types/validation";

const { parseNumber } = parsingUtils;

defineProps<{
  describedBy?: string;
  disabled?: boolean | string;
  floating?: boolean | string;
  id?: string;
  label?: string;
  max?: number | string;
  min?: number | string;
  modelValue?: number | string;
  name?: string;
  placeholder?: string;
  plaintext?: boolean | string;
  readonly?: boolean | string;
  required?: boolean | string;
  rules?: ValidationRules;
  size?: InputSize;
  status?: InputStatus;
  step?: number | string;
}>();

defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();
</script>

<template>
  <AppInput
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :max="max"
    :min="min"
    :model-value="modelValue?.toString()"
    :name="name"
    :placeholder="placeholder"
    :plaintext="plaintext"
    :readonly="readonly"
    :required="required"
    :rules="rules"
    :size="size"
    :status="status"
    :step="step"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  >
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
      <slot name="append"></slot>
    </template>
    <template #after>
      <slot name="after"></slot>
    </template>
  </AppInput>
</template>
