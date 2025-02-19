<script setup lang="ts">
import { TarInput, inputUtils, type InputOptions, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils, stringUtils } from "logitar-js";
import { useField } from "vee-validate";

import type { ValidationListeners, ValidationRules } from "@/types/validation";

const { isDateTimeInput, isNumericInput, isTextualInput } = inputUtils;
const { isNullOrWhiteSpace } = stringUtils;
const { parseBoolean, parseNumber } = parsingUtils;

const props = withDefaults(
  defineProps<
    InputOptions & {
      rules?: ValidationRules;
    }
  >(),
  {
    id: () => nanoid(),
  },
);

const inputRef = ref<InstanceType<typeof TarInput> | null>(null);

const describedBy = computed<string>(() => {
  const ids: string[] = [];
  if (props.describedBy) {
    ids.push(props.describedBy);
  }
  ids.push(`${props.id}_invalid-feedback`);
  return ids.join(" ");
});
const inputMax = computed<number | string | undefined>(() => (isDateTimeInput(props.type) ? props.max : undefined));
const inputMin = computed<number | string | undefined>(() => (isDateTimeInput(props.type) ? props.min : undefined));
const inputName = computed<string>(() => props.name ?? props.id);
const inputRequired = computed<false | "label">(() => (parseBoolean(props.required) ? "label" : false));

const validationRules = computed<ValidationRules>(() => {
  const rules: ValidationRules = {};

  const required: boolean | undefined = parseBoolean(props.required);
  if (required) {
    rules.required = true;
  }

  const max: number | undefined = parseNumber(props.max);
  const min: number | undefined = parseNumber(props.min);
  if (isNumericInput(props.type)) {
    if (typeof max === "number") {
      rules.max_value = max;
    }
    if (typeof min === "number") {
      rules.min_value = min;
    }
  } else if (isTextualInput(props.type)) {
    if (max) {
      rules.max_length = max;
    }
    if (min) {
      rules.min_length = min;
    }
  }

  if (!isNullOrWhiteSpace(props.pattern)) {
    rules.regex = props.pattern;
  }

  switch (props.type) {
    case "email":
      rules.email = true;
      break;
    case "url":
      rules.url = true;
      break;
  }

  return { ...rules, ...props.rules };
});
const displayLabel = computed<string>(() => (props.label ? props.label.toLowerCase() : inputName.value));
const { errorMessage, handleChange, meta, value } = useField<string>(inputName, validationRules, {
  initialValue: props.modelValue,
  label: displayLabel,
});
const inputStatus = computed<InputStatus | undefined>(() => {
  if (meta.dirty || meta.touched) {
    return props.status ?? (meta.valid ? "valid" : "invalid");
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
  inputRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarInput
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :max="inputMax"
    :min="inputMin"
    :model-value="value"
    :name="name"
    :placeholder="placeholder"
    :plaintext="plaintext"
    :readonly="readonly"
    ref="inputRef"
    :required="inputRequired"
    :size="size"
    :status="inputStatus"
    :step="step"
    :type="type"
    v-on="validationListeners"
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
      <div v-if="errorMessage" class="invalid-feedback" :id="describedBy">{{ errorMessage }}</div>
      <slot name="after"></slot>
    </template>
  </TarInput>
</template>
