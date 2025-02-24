<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import CustomAttributeEdit from "./CustomAttributeEdit.vue";
import type { CustomAttribute, CustomAttributeState } from "@/types/custom";
import { getCustomAttributeStatus, hasCustomAttributeChanged, toCustomAttribute, toCustomAttributeState } from "@/helpers/custom";

const { t } = useI18n();

const props = defineProps<{
  customAttributes: CustomAttribute[];
  save: (customAttributes: CustomAttribute[]) => Promise<void>;
}>();

const states = ref<CustomAttributeState[]>([]);

const hasChanges = computed<boolean>(() => states.value.some(hasCustomAttributeChanged));

function onAdd(): void {
  states.value.push({ oldKey: "", oldValue: "", newKey: "", newValue: "", isDeleted: false });
}
function onRemove(index: number): void {
  const state: CustomAttributeState | undefined = states.value[index];
  if (state && !state.isDeleted) {
    if (getCustomAttributeStatus(state) === "created") {
      states.value.splice(index, 1);
    } else {
      state.isDeleted = true;
    }
  }
}
function onRestore(index: number): void {
  const state: CustomAttributeState | undefined = states.value[index];
  if (state && state.isDeleted) {
    state.isDeleted = false;
  }
}
function onKeyUpdate(index: number, newKey: string): void {
  const state: CustomAttributeState | undefined = states.value[index];
  if (state && state.newKey !== newKey) {
    state.newKey = newKey;
  }
}
function onValueUpdate(index: number, newValue: string): void {
  const state: CustomAttributeState | undefined = states.value[index];
  if (state && state.newValue !== newValue) {
    state.newValue = newValue;
  }
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(
  async () => await props.save(states.value.filter((customAttribute) => hasCustomAttributeChanged(customAttribute)).map(toCustomAttribute)),
);

watch(
  () => props.customAttributes,
  (customAttributes) => (states.value = customAttributes.map(toCustomAttributeState)),
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="onSubmit">
    <div class="mb-3">
      <TarButton icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="onAdd" />
    </div>
    <CustomAttributeEdit
      v-for="(state, index) in states"
      :key="index"
      :attribute-key="state.newKey"
      :deleted="state.isDeleted"
      :id="`custom-attribute-${index}`"
      :value="state.newValue"
      @remove="onRemove(index)"
      @restore="onRestore(index)"
      @update:key="onKeyUpdate(index, $event)"
      @update:value="onValueUpdate(index, $event)"
    />
    <div v-if="states.length > 0" class="mb-3">
      <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
    </div>
  </form>
</template>
