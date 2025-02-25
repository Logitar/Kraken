<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import type { ApiKey, CreateOrReplaceApiKeyPayload } from "@/types/apiKeys";
import { createApiKey } from "@/api/apiKeys";

const { t } = useI18n();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  name.value = "";
}

const emit = defineEmits<{
  (e: "created", value: ApiKey): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  reset();
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: CreateOrReplaceApiKeyPayload = {
      name: name.value,
      customAttributes: [],
      roles: [],
    };
    const apiKey: ApiKey = await createApiKey(payload);
    emit("created", apiKey);
    reset();
    hide();
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-apiKey" />
    <TarModal :close="t('actions.close')" id="create-apiKey" ref="modalRef" :title="t('apiKeys.create')">
      <form>
        <DisplayNameInput id="name" label="apiKeys.name" required v-model="name" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting"
          icon="fas fa-plus"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('actions.create')"
          type="submit"
          variant="success"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
