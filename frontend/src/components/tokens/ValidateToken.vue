<script setup lang="ts">
import { TarAlert, TarButton, TarCheckbox } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import AudienceInput from "./AudienceInput.vue";
import IssuerInput from "./IssuerInput.vue";
import SecretInput from "./SecretInput.vue";
import TokenTypeInput from "./TokenTypeInput.vue";
import ValidatedToken from "./ValidatedToken.vue";
import type { ApiError, Error as ErrorT, ProblemDetails } from "@/types/api";
import type { ValidatedToken as ValidatedTokenT, ValidateTokenPayload } from "@/types/tokens";
import { validateToken } from "@/api/tokens";
import { StatusCodes } from "@/enums/statusCodes";

const { t } = useI18n();

const audience = ref<string>("");
const consume = ref<boolean>(false);
const error = ref<ErrorT>();
const issuer = ref<string>("");
const secret = ref<string>("");
const token = ref<string>("");
const type = ref<string>("");
const validatedToken = ref<ValidatedTokenT>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  error.value = undefined;
  validatedToken.value = undefined;
  try {
    const payload: ValidateTokenPayload = {
      token: token.value,
      consume: consume.value,
      audience: audience.value,
      issuer: issuer.value,
      secret: secret.value,
      type: type.value,
    };
    validatedToken.value = await validateToken(payload);
  } catch (e: unknown) {
    const { data, status } = e as ApiError;
    if (status === StatusCodes.BadRequest) {
      const details = data as ProblemDetails;
      if (details.error) {
        error.value = details.error;
        return;
      }
    }
    emit("error", e);
  }
});
</script>

<template>
  <div>
    <form @submit.prevent="onSubmit">
      <div class="mb-3">
        <TarCheckbox id="consume" :label="t('tokens.consume')" v-model="consume" />
      </div>
      <AppInput class="mb-3" floating id="token" :label="t('tokens.token')" :placeholder="t('tokens.token')" required v-model="token" />
      <div class="mb-3 row">
        <SecretInput class="col" v-model="secret" />
        <TokenTypeInput class="col" v-model="type" />
      </div>
      <div class="mb-3 row">
        <AudienceInput class="col" v-model="audience" />
        <IssuerInput class="col" v-model="issuer" />
      </div>
      <div class="mb-3">
        <TarButton
          :disabled="isSubmitting"
          icon="fas fa-id-card"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('actions.validate')"
          type="submit"
          variant="primary"
        />
      </div>
    </form>
    <TarAlert v-if="error" show variant="danger">
      <strong>{{ error.code }}</strong> {{ error.message }}
    </TarAlert>
    <ValidatedToken v-else-if="validatedToken" :token="validatedToken" />
  </div>
</template>
