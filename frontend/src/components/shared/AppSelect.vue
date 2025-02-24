<script setup lang="ts">
import { TarSelect, type SelectOptions, type SelectStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useField } from "vee-validate";

import type { ValidationListeners, ValidationRules } from "@/types/validation";

const { parseBoolean } = parsingUtils;

const props = withDefaults(
  defineProps<
    SelectOptions & {
      rules?: ValidationRules;
    }
  >(),
  {
    id: () => nanoid(),
  },
);

const selectRef = ref<InstanceType<typeof TarSelect> | null>(null);

const describedBy = computed<string>(() => {
  const ids: string[] = [];
  if (props.describedBy) {
    ids.push(props.describedBy);
  }
  ids.push(`${props.id}_invalid-feedback`);
  return ids.join(" ");
});
const inputName = computed<string>(() => props.name ?? props.id);
const inputRequired = computed<false | "label">(() => (parseBoolean(props.required) ? "label" : false));

const validationRules = computed<ValidationRules>(() => {
  const rules: ValidationRules = {};

  const required: boolean | undefined = parseBoolean(props.required);
  if (required) {
    rules.required = true;
  }

  return { ...rules, ...props.rules };
});
const displayLabel = computed<string>(() => (props.label ? props.label.toLowerCase() : inputName.value));
const { errorMessage, handleChange, meta, value } = useField<string>(inputName, validationRules, {
  initialValue: props.modelValue,
  label: displayLabel,
});
const inputStatus = computed<SelectStatus | undefined>(() => {
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
  selectRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarSelect
    :aria-label="ariaLabel"
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :model-value="value"
    :multiple="multiple"
    :name="name"
    :options="options"
    :placeholder="placeholder"
    ref="selectRef"
    :required="inputRequired"
    :size="size"
    :status="inputStatus"
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
    <template #placeholder-override>
      <slot name="placeholder-override"></slot>
    </template>
    <template #append>
      <slot name="append"></slot>
    </template>
    <template #after>
      <div v-if="errorMessage" class="invalid-feedback" :id="describedBy">{{ errorMessage }}</div>
      <slot name="after"></slot>
    </template>
  </TarSelect>
</template>
