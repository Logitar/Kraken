<script setup lang="ts">
import { TarTextarea, type TextareaOptions, type TextareaStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useField } from "vee-validate";

import type { ValidationListeners, ValidationRules, ValidationType } from "@/types/validation";

const { parseBoolean, parseNumber } = parsingUtils;

const props = withDefaults(
  defineProps<
    TextareaOptions & {
      rules?: ValidationRules;
      validation?: ValidationType;
    }
  >(),
  {
    id: () => nanoid(),
    validation: "client",
  },
);

const textareaRef = ref<InstanceType<typeof TarTextarea> | null>(null);

const describedBy = computed<string>(() => {
  const ids: string[] = [];
  if (props.describedBy) {
    ids.push(props.describedBy);
  }
  ids.push(`${props.id}_invalid-feedback`);
  return ids.join(" ");
});
const inputName = computed<string>(() => props.name ?? props.id);
const inputRequired = computed<boolean | "label">(() => (parseBoolean(props.required) ? (props.validation === "server" ? true : "label") : false));

const validationRules = computed<ValidationRules>(() => {
  const rules: ValidationRules = {};
  if (props.validation === "server") {
    return rules;
  }

  const required: boolean | undefined = parseBoolean(props.required);
  if (required) {
    rules.required = true;
  }

  const max: number | undefined = parseNumber(props.max);
  if (max) {
    rules.max_length = max;
  }
  const min: number | undefined = parseNumber(props.min);
  if (min) {
    rules.min_length = min;
  }

  return { ...rules, ...props.rules };
});
const displayLabel = computed<string>(() => (props.label ? props.label.toLowerCase() : inputName.value));
const { errorMessage, handleChange, meta, value } = useField<string>(inputName, validationRules, {
  initialValue: props.modelValue,
  label: displayLabel,
});
const inputStatus = computed<TextareaStatus | undefined>(() => {
  if (meta.dirty || meta.touched) {
    return props.status ?? (props.validation === "server" ? undefined : meta.valid ? "valid" : "invalid");
  }
  return undefined;
});
const validationListeners = computed<ValidationListeners>(() => ({
  blur: handleChange,
  change: handleChange,
  input: errorMessage.value ? handleChange : (e: unknown) => handleChange(e, false),
}));

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function focus(): void {
  textareaRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarTextarea
    :cols="cols"
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :max="validation === 'server' ? max : undefined"
    :min="validation === 'server' ? min : undefined"
    :model-value="validation === 'server' ? modelValue : value"
    :name="name"
    :placeholder="placeholder"
    :plaintext="plaintext"
    :readonly="readonly"
    ref="textareaRef"
    :required="inputRequired"
    :rows="rows"
    :size="size"
    :status="inputStatus"
    v-on="validationListeners"
  >
    <template #before>
      <slot name="before"></slot>
    </template>
    <template #label-override>
      <slot name="label-override"></slot>
    </template>
    <template #label-required>
      <slot name="label-required"></slot>
    </template>
    <template #after>
      <div v-if="errorMessage" class="invalid-feedback" :id="describedBy">{{ errorMessage }}</div>
      <slot name="after"></slot>
    </template>
  </TarTextarea>
</template>
