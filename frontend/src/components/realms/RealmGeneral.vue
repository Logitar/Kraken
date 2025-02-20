<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import UniqueSlugAlreadyUsed from "./UniqueSlugAlreadyUsed.vue";
import UniqueSlugInput from "./UniqueSlugInput.vue";
import type { Realm, UpdateRealmPayload } from "@/types/realms";
import type { UniqueNameSettings } from "@/types/settings";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { isError } from "@/helpers/errors";
import { updateRealm } from "@/api/realms";

const props = defineProps<{
  realm: Realm;
  settings: UniqueNameSettings;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const uniqueSlug = ref<string>("");
const uniqueSlugAlreadyUsed = ref<boolean>(false);
const url = ref<string>("");

const hasChanges = computed<boolean>(
  () =>
    (props.realm.displayName ?? "") !== displayName.value ||
    props.realm.uniqueSlug !== uniqueSlug.value ||
    (props.realm.description ?? "") !== description.value ||
    (props.realm.url ?? "") !== url.value,
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Realm): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueSlugAlreadyUsed.value = false;
  try {
    const payload: UpdateRealmPayload = {
      uniqueSlug: props.realm.uniqueSlug !== uniqueSlug.value ? uniqueSlug.value : undefined,
      displayName: (props.realm.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
      description: (props.realm.description ?? "") !== description.value ? { value: description.value } : undefined,
      url: (props.realm.url ?? "") !== url.value ? { value: url.value } : undefined,
      customAttributes: [],
    };
    const realm: Realm = await updateRealm(props.realm.id, payload);
    emit("updated", realm);
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueSlugAlreadyUsed)) {
      uniqueSlugAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
});

watch(
  () => props.realm,
  (realm) => {
    displayName.value = realm.displayName ?? "";
    uniqueSlug.value = realm.uniqueSlug;
    description.value = realm.description ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <form @submit.prevent="onSubmit">
      <UniqueSlugAlreadyUsed v-model="uniqueSlugAlreadyUsed" />
      <div class="mb-3 row">
        <DisplayNameInput class="col" v-model="displayName" />
        <UniqueSlugInput class="col" :name-value="displayName" required v-model="uniqueSlug" />
      </div>
      <DescriptionTextarea class="mb-3" v-model="description" />
      <div class="mb-3">
        <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
      </div>
    </form>
  </div>
</template>
