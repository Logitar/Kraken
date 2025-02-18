<script setup lang="ts">
import { TarCheckbox, type CheckboxOptions } from "logitar-vue3-ui";
import { computed } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";

const { parseBoolean } = parsingUtils;

const props = withDefaults(
  defineProps<
    CheckboxOptions & {
      tight?: boolean | string;
    }
  >(),
  {
    id: () => nanoid(),
  },
);

const isTight = computed<boolean>(() => parseBoolean(props.tight) ?? false);

defineEmits<{
  (e: "update:model-value", value: boolean): void;
}>();
</script>

<template>
  <div :class="{ 'mb-3': !isTight }">
    <TarCheckbox
      :aria-label="ariaLabel"
      :disabled="disabled"
      :id="id"
      :inline="inline"
      :label="label"
      :model-value="modelValue"
      :name="name"
      :required="required"
      :reverse="reverse"
      :role="role"
      :switch="props.switch"
      :value="value"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <slot name="after"></slot>
  </div>
</template>
