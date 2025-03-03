<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import ClaimEdit from "./ClaimEdit.vue";
import type { Claim } from "@/types/tokens";

const { t } = useI18n();

const props = defineProps<{
  modelValue: Claim[];
}>();

const emit = defineEmits<{
  (e: "update:model-value", claim: Claim[]): void;
}>();

function onAdd(): void {
  const claim: Claim = { name: "", value: "" };
  const modelValue: Claim[] = [...props.modelValue, claim];
  emit("update:model-value", modelValue);
}
function onRemove(index: number): void {
  const modelValue: Claim[] = [...props.modelValue];
  modelValue.splice(index, 1);
  emit("update:model-value", modelValue);
}
function onUpdate(index: number, claim: Claim): void {
  const modelValue: Claim[] = [...props.modelValue];
  modelValue.splice(index, 1, claim);
  emit("update:model-value", modelValue);
}
</script>

<template>
  <div>
    <div class="mb-3">
      <TarButton icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="onAdd" />
    </div>
    <ClaimEdit
      v-for="(claim, index) in modelValue"
      :key="index"
      :claim="claim"
      :id="`claim-${index}`"
      @removed="onRemove(index)"
      @updated="onUpdate(index, $event)"
    />
  </div>
</template>
