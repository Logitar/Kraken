<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import RoleCard from "./RoleCard.vue";
import RoleSelect from "./RoleSelect.vue";
import type { Role } from "@/types/roles";

const { t } = useI18n();

const props = defineProps<{
  add: (role: Role) => Promise<void>;
  remove: (role: Role) => Promise<void>;
  roles: Role[];
}>();

const isAdding = ref<boolean>(false);
const isRemoving = ref<Set<string>>(new Set<string>());
const role = ref<Role>();

const excluded = computed<string[]>(() => props.roles.map(({ id }) => id));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

async function onAdd(): Promise<void> {
  if (role.value && !isAdding.value) {
    isAdding.value = true;
    try {
      await props.add(role.value);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      role.value = undefined;
      isAdding.value = false;
    }
  }
}
async function onRemove(role: Role): Promise<void> {
  if (!isRemoving.value.has(role.id)) {
    isRemoving.value.add(role.id);
    try {
      await props.remove(role);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isRemoving.value.delete(role.id);
    }
  }
}
</script>

<template>
  <div>
    <RoleSelect class="mb-3" :exclude="excluded" :model-value="role?.id" @error="$emit('error', $event)" @selected="role = $event">
      <template #append>
        <TarButton :disabled="!role" icon="fas fa-plus" :loading="isAdding" :text="t('actions.add')" variant="success" @click="onAdd" />
      </template>
    </RoleSelect>
    <div v-if="roles.length > 0" class="row">
      <div v-for="role in roles" :key="role.id" class="col-sm-6 col-md-4 col-lg-3 mb-3">
        <RoleCard :loading="isRemoving.has(role.id)" remove :role="role" @removed="onRemove(role)" />
      </div>
    </div>
  </div>
</template>
