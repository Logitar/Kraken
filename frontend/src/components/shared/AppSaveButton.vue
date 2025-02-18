<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{
  disabled?: boolean;
  loading?: boolean;
}>();
</script>

<template>
  <TarButton :disabled="disabled" icon="fas fa-save" :loading="loading" :status="t('loading')" :text="t('actions.save')" type="submit" variant="primary" />
</template>
