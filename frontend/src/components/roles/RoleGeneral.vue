<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Role, UpdateRolePayload } from "@/types/roles";
import type { UniqueNameSettings } from "@/types/settings";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { isError } from "@/helpers/errors";
import { updateRole } from "@/api/roles";

const props = defineProps<{
  role: Role;
  settings: UniqueNameSettings;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(
  () =>
    props.role.uniqueName !== uniqueName.value || (props.role.displayName ?? "") !== displayName.value || (props.role.description ?? "") !== description.value,
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Role): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
  try {
    const payload: UpdateRolePayload = {
      uniqueName: props.role.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
      displayName: (props.role.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
      description: (props.role.description ?? "") !== description.value ? { value: description.value } : undefined,
      customAttributes: [],
    };
    const role: Role = await updateRole(props.role.id, payload);
    emit("updated", role);
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
      uniqueNameAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
});

watch(
  () => props.role,
  (role) => {
    uniqueName.value = role.uniqueName;
    displayName.value = role.displayName ?? "";
    description.value = role.description ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <form @submit.prevent="onSubmit">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <div class="mb-3 row">
        <UniqueNameInput class="col" required :settings="settings" v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <DescriptionTextarea class="mb-3" v-model="description" />
      <div class="mb-3">
        <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
      </div>
    </form>
  </div>
</template>
