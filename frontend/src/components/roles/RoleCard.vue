<script setup lang="ts">
import { TarButton, TarCard } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { Role } from "@/types/roles";

const { t } = useI18n();

defineProps<{
  loading?: boolean | string;
  remove?: boolean;
  role: Role;
}>();

const emit = defineEmits<{
  (e: "removed"): void;
}>();

function onRemove(): void {
  emit("removed");
}
</script>

<template>
  <TarCard :title="role.uniqueName" :subtitle="role.displayName">
    <div v-if="remove" class="float-end">
      <TarButton
        :disabled="loading"
        icon="fas fa-times"
        :loading="loading"
        :status="t('loading')"
        :text="t('actions.remove')"
        variant="danger"
        @click="onRemove"
      />
    </div>
  </TarCard>
</template>
