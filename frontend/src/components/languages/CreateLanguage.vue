<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import LocaleAlreadyUsed from "./LocaleAlreadyUsed.vue";
import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import type { CreateOrReplaceLanguagePayload, Language } from "@/types/languages";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { createLanguage } from "@/api/languages";
import { isError } from "@/helpers/errors";

const { t } = useI18n();

const locale = ref<string>("");
const localeAlreadyUsed = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  localeAlreadyUsed.value = false;
  locale.value = "";
}

const emit = defineEmits<{
  (e: "created", value: Language): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  reset();
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  localeAlreadyUsed.value = false;
  try {
    const payload: CreateOrReplaceLanguagePayload = {
      locale: locale.value,
    };
    const language: Language = await createLanguage(payload);
    emit("created", language);
    reset();
    hide();
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.LocaleAlreadyUsed)) {
      localeAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-language" />
    <TarModal :close="t('actions.close')" id="create-language" ref="modalRef" :title="t('languages.create')">
      <LocaleAlreadyUsed v-model="localeAlreadyUsed" />
      <form>
        <LocaleSelect required v-model="locale" />
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
