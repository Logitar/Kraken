<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useRoute, useRouter } from "vue-router";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceRolePayload, Role } from "@/types/roles";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { handleErrorKey } from "@/inject/App";
import { isError } from "@/helpers/errors";
import { readRole, replaceRole } from "@/api/roles";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const description = ref<string>("");
const displayName = ref<string>("");
const role = ref<Role>();
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(() =>
  Boolean(
    role.value &&
      (role.value.uniqueName !== uniqueName.value ||
        (role.value.displayName ?? "") !== displayName.value ||
        (role.value.description ?? "" !== description.value)),
  ),
);

function setModel(model: Role): void {
  role.value = model;
  uniqueName.value = model.uniqueName;
  displayName.value = model.displayName ?? "";
  description.value = model.description ?? "";
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
  if (role.value) {
    try {
      const payload: CreateOrReplaceRolePayload = {
        uniqueName: uniqueName.value,
        displayName: displayName.value,
        description: description.value,
        customAttributes: role.value.customAttributes, // TODO(fpion): implement
      };
      const updatedRole: Role = await replaceRole(role.value.id, payload, role.value.version);
      setModel(updatedRole);
      toasts.success("roles.updated");
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.LocaleAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
      } else {
        handleError(e);
      }
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const role: Role = await readRole(id);
      setModel(role);
    }
    // TODO(fpion): load realm or configuration, for unique name settings
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
    <template v-if="role">
      <h1>{{ role.displayName ?? role.uniqueName }}</h1>
      <StatusDetail :aggregate="role" />
      <form @submit.prevent="onSubmit">
        <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
        <div class="row">
          <UniqueNameInput class="col" required v-model="uniqueName" />
          <DisplayNameInput class="col" v-model="displayName" />
        </div>
        <DescriptionTextarea v-model="description" />
        <div class="mb-3">
          <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
        </div>
      </form>
    </template>
  </main>
</template>
