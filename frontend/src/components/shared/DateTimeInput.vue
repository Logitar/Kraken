<script setup lang="ts">
import type { InputSize, InputStatus } from "logitar-vue3-ui";
import { dateUtils } from "logitar-js";

import AppInput from "./AppInput.vue";
import type { ValidationRules } from "@/types/validation";

const { toDateTimeLocal } = dateUtils;

defineProps<{
  describedBy?: string;
  disabled?: boolean | string;
  floating?: boolean | string;
  id?: string;
  label?: string;
  max?: Date;
  min?: Date;
  modelValue?: Date;
  name?: string;
  plaintext?: boolean | string;
  readonly?: boolean | string;
  required?: boolean | string;
  rules?: ValidationRules;
  size?: InputSize;
  status?: InputStatus;
  step?: number | string;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value?: Date): void;
}>();
function onModelValueUpdate(value: string): void {
  try {
    const date = new Date(value);
    emit("update:model-value", isNaN(date.getTime()) ? undefined : date);
  } catch (e: unknown) {
    emit("update:model-value", undefined);
  }
}
</script>

<template>
  <AppInput
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :max="max ? toDateTimeLocal(max) : undefined"
    :min="min ? toDateTimeLocal(min) : undefined"
    :model-value="modelValue ? toDateTimeLocal(modelValue) : undefined"
    :name="name"
    :plaintext="plaintext"
    :readonly="readonly"
    :required="required"
    :rules="rules"
    :size="size"
    :step="step"
    type="datetime-local"
    @update:model-value="onModelValueUpdate"
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
