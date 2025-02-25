<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DateTimeInput from "@/components/shared/DateTimeInput.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import type { ApiKey, UpdateApiKeyPayload } from "@/types/keys";
import { updateApiKey } from "@/api/keys";

const { t } = useI18n();

const props = defineProps<{
  apiKey: ApiKey;
}>();

const description = ref<string>("");
const expiresOn = ref<Date>();
const name = ref<string>("");

const hasChanges = computed<boolean>(
  () =>
    props.apiKey.name !== name.value ||
    (props.apiKey.description ?? "") !== description.value ||
    (props.apiKey.expiresOn ? new Date(props.apiKey.expiresOn).getTime() : -1) !== (expiresOn.value?.getTime() ?? -1),
);
const max = computed<Date | undefined>(() => (props.apiKey.expiresOn ? new Date(props.apiKey.expiresOn) : undefined));
const min = computed<Date>(() => {
  const expiresOn: Date | undefined = props.apiKey.expiresOn ? new Date(props.apiKey.expiresOn) : undefined;
  const now: Date = new Date();
  return expiresOn && expiresOn < now ? expiresOn : now;
});

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: ApiKey): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: UpdateApiKeyPayload = {
      name: props.apiKey.name !== name.value ? name.value : undefined,
      description: (props.apiKey.description ?? "") !== description.value ? { value: description.value } : undefined,
      expiresOn: (props.apiKey.expiresOn ?? "") !== (expiresOn.value?.toISOString() ?? "") ? expiresOn.value?.toISOString() : undefined,
      customAttributes: [],
      roles: [],
    };
    const apiKey: ApiKey = await updateApiKey(props.apiKey.id, payload);
    emit("updated", apiKey);
  } catch (e: unknown) {
    emit("error", e);
  }
});

watch(
  () => props.apiKey,
  (apiKey) => {
    name.value = apiKey.name;
    description.value = apiKey.description ?? "";
    expiresOn.value = apiKey.expiresOn ? new Date(apiKey.expiresOn) : undefined;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <form @submit.prevent="onSubmit">
      <div class="mb-3 row">
        <DisplayNameInput class="col" id="name" label="apiKeys.name" required v-model="name" />
        <DateTimeInput
          class="col"
          floating
          id="expires-on"
          :label="t('apiKeys.expiresOn')"
          :min="min"
          :max="max"
          :required="Boolean(apiKey.expiresOn)"
          v-model="expiresOn"
        />
      </div>
      <DescriptionTextarea class="mb-3" v-model="description" />
      <div class="mb-3">
        <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
      </div>
    </form>
  </div>
</template>
