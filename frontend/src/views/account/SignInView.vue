<script setup lang="ts">
import { TarAlert, TarButton } from "logitar-vue3-ui";
import { inject, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import PasswordInput from "@/components/users/PasswordInput.vue";
import UsernameInput from "@/components/users/UsernameInput.vue";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { handleErrorKey } from "@/inject/App";
import { isError } from "@/helpers/errors";
import { signIn } from "@/api/account";
import { useAccountStore } from "@/stores/account";
import type { SignInPayload } from "@/types/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const invalidCredentials = ref<boolean>(false);
const password = ref<string>("");
const passwordRef = ref<InstanceType<typeof PasswordInput> | null>(null);
const username = ref<string>("");

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    invalidCredentials.value = false;
    const payload: SignInPayload = { username: username.value, password: password.value };
    const currentUser = await signIn(payload);
    account.signIn(currentUser);
    const redirect: string | undefined = route.query.redirect?.toString();
    router.push(redirect ?? { name: "Profile" });
  } catch (e: unknown) {
    if (isError(e, StatusCodes.BadRequest, ErrorCodes.InvalidCredentials)) {
      invalidCredentials.value = true;
      password.value = "";
      passwordRef.value?.focus();
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <h1>{{ t("users.signIn.title") }}</h1>
    <TarAlert :close="t('actions.close')" dismissible variant="warning" v-model="invalidCredentials">
      <strong>{{ t("users.signIn.failed") }}</strong> {{ t("users.signIn.invalidCredentials") }}
    </TarAlert>
    <form @submit.prevent="onSubmit">
      <UsernameInput class="mb-3" required v-model="username" />
      <PasswordInput class="mb-3" required ref="passwordRef" v-model="password" />
      <TarButton
        :disabled="isSubmitting"
        icon="fas fa-arrow-right-to-bracket"
        :loading="isSubmitting"
        :status="t('loading')"
        :text="t('users.signIn.submit')"
        type="submit"
      />
    </form>
  </main>
</template>
