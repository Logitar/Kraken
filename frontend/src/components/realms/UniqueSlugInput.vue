<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";

const { slugify } = stringUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    nameValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "unique-slug",
    label: "realms.uniqueSlug.label",
  },
);

const isCustom = ref<boolean>(props.modelValue !== slugify(props.nameValue));

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onCustomUpdate(value: boolean): void {
  // TODO(fpion): not working correctly because of VeeValidate!
  isCustom.value = value;
  if (!value) {
    emit("update:model-value", slugify(props.nameValue));
  }
}

watch(
  () => props.nameValue,
  (name) => {
    if (!isCustom.value) {
      emit("update:model-value", slugify(name));
    }
  },
  { immediate: true },
);
</script>

<template>
  <AppInput
    :disabled="!isCustom"
    floating
    :id="id"
    :label="t(label)"
    max="255"
    :model-value="modelValue"
    :placeholder="t(label)"
    :required="required"
    :rules="{ slug: true }"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <TarCheckbox :id="`${id}-custom`" :label="t('realms.uniqueSlug.custom')" :model-value="isCustom" @update:model-value="onCustomUpdate" />
    </template>
  </AppInput>
</template>
