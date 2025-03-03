<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  attributeKey: string;
  deleted?: boolean | string;
  id: string;
  value: string;
}>();

const isDeleted = computed<boolean>(() => parseBoolean(props.deleted) ?? false);

defineEmits<{
  (e: "remove"): void;
  (e: "restore"): void;
  (e: "update:key", value: string): void;
  (e: "update:value", value: string): void;
}>();
</script>

<template>
  <div class="mb-3 row">
    <AppInput
      class="col"
      floating
      :id="`${id}-key`"
      :label="t('customAttributes.key')"
      max="255"
      :model-value="attributeKey"
      :placeholder="t('customAttributes.key')"
      required
      :rules="{ identifier: true }"
      @update:model-value="$emit('update:key', $event)"
    />
    <AppInput
      class="col"
      floating
      :id="`${id}-value`"
      :label="t('customAttributes.value')"
      :model-value="value"
      :placeholder="t('customAttributes.value')"
      required
      @update:model-value="$emit('update:value', $event)"
    >
      <template #append>
        <TarButton v-if="isDeleted" icon="fas fa-ban" :text="t('actions.restore')" variant="warning" @click="$emit('restore')" />
        <TarButton v-else icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('remove')" />
      </template>
    </AppInput>
  </div>
</template>
