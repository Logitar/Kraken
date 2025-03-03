<script setup lang="ts">
import { TarButton, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import AppSelect from "@/components/shared/AppSelect.vue";
import type { Claim } from "@/types/tokens";
import claimTypes from "@/resources/claimTypes.json";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  claim: Claim;
  id: string;
}>();

const types = computed<SelectOption[]>(() =>
  orderBy(
    claimTypes.map(({ id, name }) => ({ text: name, value: id })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "updated", claim: Claim): void;
}>();

function onNameUpdate(name: string): void {
  const claim: Claim = { ...props.claim, name };
  emit("updated", claim);
}
function onTypeUpdate(type: string): void {
  const claim: Claim = { ...props.claim, type };
  emit("updated", claim);
}
function onValueUpdate(value: string): void {
  const claim: Claim = { ...props.claim, value };
  emit("updated", claim);
}
</script>

<template>
  <div class="mb-3 row">
    <AppInput
      class="col"
      floating
      :id="`${id}-name`"
      :label="t('tokens.claims.name')"
      :model-value="claim.name"
      max="255"
      :placeholder="t('tokens.claims.name')"
      required
      :rules="{ identifier: true }"
      @update:model-value="onNameUpdate"
    />
    <AppInput
      class="col"
      floating
      :id="`${id}-value`"
      :label="t('tokens.claims.value')"
      :model-value="claim.value"
      :placeholder="t('tokens.claims.value')"
      required
      @update:model-value="onValueUpdate"
    />
    <AppSelect
      class="col"
      floating
      :id="`${id}-type`"
      :label="t('tokens.claims.type')"
      :model-value="claim.type"
      :options="types"
      :placeholder="t('tokens.claims.type')"
      @update:model-value="onTypeUpdate"
    >
      <template #append>
        <TarButton icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
      </template>
    </AppSelect>
  </div>
</template>
