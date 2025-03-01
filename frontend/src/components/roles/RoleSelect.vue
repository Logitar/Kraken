<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Role, SearchRolesPayload } from "@/types/roles";
import type { SearchResults } from "@/types/search";
import { formatRole } from "@/helpers/format";
import { searchRoles } from "@/api/roles";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  exclude?: string[];
  modelValue: string | undefined;
}>();

const roles = ref<Role[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    roles.value
      .filter(({ id }) => !props.exclude || !props.exclude.includes(id))
      .map(
        (role) =>
          ({
            text: formatRole(role),
            value: role.id,
          }) as SelectOption,
      ),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: Role | undefined): void;
  (e: "update:model-value", value: string | undefined): void;
}>();

function onModelValueUpdate(id: string | undefined): void {
  emit("update:model-value", id);

  const role: Role | undefined = roles.value.find((role) => role.id === id);
  emit("selected", role);
}

onMounted(async () => {
  try {
    const payload: SearchRolesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [{ field: "DisplayName", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Role> = await searchRoles(payload);
    roles.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <TarSelect
    :disabled="options.length < 1"
    floating
    id="role"
    :label="t('roles.select.label')"
    :model-value="modelValue"
    :options="options"
    :placeholder="t('roles.select.placeholder')"
    @update:model-value="onModelValueUpdate($event)"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </TarSelect>
</template>
