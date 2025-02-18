<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useRoute, useRouter } from "vue-router";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DefaultButton from "@/components/languages/DefaultButton.vue";
import LocaleAlreadyUsed from "@/components/languages/LocaleAlreadyUsed.vue";
import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceLanguagePayload, Language } from "@/types/languages";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { handleErrorKey } from "@/inject/App";
import { isError } from "@/helpers/errors";
import { readLanguage, replaceLanguage } from "@/api/languages";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const language = ref<Language>();
const locale = ref<string>("");
const localeAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(() => Boolean(language.value && language.value.locale.code !== locale.value));

function setModel(model: Language): void {
  language.value = model;
  locale.value = model.locale.code;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  localeAlreadyUsed.value = false;
  if (language.value) {
    try {
      const payload: CreateOrReplaceLanguagePayload = {
        locale: locale.value,
      };
      const updatedLanguage: Language = await replaceLanguage(language.value.id, payload, language.value.version);
      setModel(updatedLanguage);
      toasts.success("languages.updated");
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.LocaleAlreadyUsed)) {
        localeAlreadyUsed.value = true;
      } else {
        handleError(e);
      }
    }
  }
});

function onSetDefault(language: Language): void {
  setModel(language);
  toasts.success("languages.default.set");
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const language: Language = await readLanguage(id);
      setModel(language);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === StatusCodes.NotFound) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="language">
      <h1>{{ language.locale.nativeName }}</h1>
      <StatusDetail :aggregate="language" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <DefaultButton :language="language" @error="handleError" @saved="onSetDefault" />
        </div>
        <LocaleAlreadyUsed v-model="localeAlreadyUsed" />
        <LocaleSelect required v-model="locale" />
        <div class="mb-3">
          <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
        </div>
      </form>
    </template>
  </main>
</template>
