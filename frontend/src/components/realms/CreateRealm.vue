<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import UniqueSlugAlreadyUsed from "./UniqueSlugAlreadyUsed.vue";
import UniqueSlugInput from "./UniqueSlugInput.vue";
import type { CreateOrReplaceRealmPayload, Realm } from "@/types/realms";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { createRealm } from "@/api/realms";
import { isError } from "@/helpers/errors";

const { t } = useI18n();

const displayName = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueSlug = ref<string>("");
const uniqueSlugAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  uniqueSlugAlreadyUsed.value = false;
  displayName.value = "";
  uniqueSlug.value = "";
}

const emit = defineEmits<{
  (e: "created", value: Realm): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  reset();
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueSlugAlreadyUsed.value = false;
  try {
    const payload: CreateOrReplaceRealmPayload = {
      uniqueSlug: uniqueSlug.value,
      displayName: displayName.value,
      uniqueNameSettings: undefined!,
      passwordSettings: undefined!,
      requireUniqueEmail: true,
      requireConfirmedAccount: true,
      customAttributes: [],
    };
    const realm: Realm = await createRealm(payload);
    emit("created", realm);
    reset();
    hide();
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueSlugAlreadyUsed)) {
      uniqueSlugAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-realm" />
    <TarModal :close="t('actions.close')" id="create-realm" ref="modalRef" :title="t('realms.create')">
      <UniqueSlugAlreadyUsed v-model="uniqueSlugAlreadyUsed" />
      <form>
        <DisplayNameInput class="mb-3" v-model="displayName" />
        <UniqueSlugInput class="mb-3" :name-value="displayName" required v-model="uniqueSlug" />
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
