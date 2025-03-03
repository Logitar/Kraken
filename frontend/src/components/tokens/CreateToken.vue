<script setup lang="ts">
import { TarButton, TarCheckbox } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AudienceInput from "./AudienceInput.vue";
import ClaimList from "./ClaimList.vue";
import CreatedToken from "./CreatedToken.vue";
import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import IssuerInput from "./IssuerInput.vue";
import LifetimeInput from "./LifetimeInput.vue";
import SecretInput from "./SecretInput.vue";
import SubjectInput from "./SubjectInput.vue";
import TokenTypeInput from "./TokenTypeInput.vue";
import type { Claim, CreatedToken as CreatedTokenT, CreateTokenPayload } from "@/types/tokens";
import type { EmailPayload } from "@/types/users";
import { createToken } from "@/api/tokens";

const { t } = useI18n();

const audience = ref<string>("");
const claims = ref<Claim[]>([]);
const createdToken = ref<CreatedTokenT>();
const email = ref<EmailPayload>({ address: "", isVerified: false });
const isConsumable = ref<boolean>(false);
const issuer = ref<string>("");
const lifetimeSeconds = ref<number>(0);
const secret = ref<string>("");
const subject = ref<string>("");
const type = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: CreateTokenPayload = {
      isConsumable: isConsumable.value,
      audience: audience.value,
      issuer: issuer.value,
      lifetimeSeconds: lifetimeSeconds.value || undefined,
      secret: secret.value,
      type: type.value,
      subject: subject.value,
      email: email.value.address ? email.value : undefined,
      claims: claims.value,
    };
    createdToken.value = await createToken(payload);
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <div>
    <form @submit.prevent="onSubmit">
      <div class="mb-3">
        <TarCheckbox id="is-consumable" :label="t('tokens.consumable')" v-model="isConsumable" />
      </div>
      <LifetimeInput class="mb-3" v-model="lifetimeSeconds" />
      <div class="mb-3 row">
        <SecretInput class="col" v-model="secret" />
        <TokenTypeInput class="col" v-model="type" />
      </div>
      <div class="mb-3 row">
        <AudienceInput class="col" v-model="audience" />
        <IssuerInput class="col" v-model="issuer" />
      </div>
      <h5>{{ t("tokens.claims.label") }}</h5>
      <div class="mb-3 row">
        <SubjectInput class="col" v-model="subject" />
        <EmailAddressInput class="col" v-model="email.address">
          <template #append>
            <div class="input-group-text">
              <!-- TODO(fpion): not working correctly because of VeeValidate! -->
              <TarCheckbox id="is-email-verified" :label="t('users.email.verified')" v-model="email.isVerified" />
            </div>
          </template>
        </EmailAddressInput>
      </div>
      <ClaimList v-model="claims" />
      <div class="mb-3">
        <TarButton
          :disabled="isSubmitting"
          icon="fas fa-id-card"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('actions.create')"
          type="submit"
          variant="primary"
        />
      </div>
    </form>
    <CreatedToken v-if="createdToken" :token="createdToken.token" />
  </div>
</template>
