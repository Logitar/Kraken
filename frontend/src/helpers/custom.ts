import type { CustomAttribute, CustomAttributeState, CustomAttributeStatus } from "@/types/custom";

export function getCustomAttributeStatus(state: CustomAttributeState): CustomAttributeStatus | undefined {
  if (!state.oldKey || !state.oldValue) {
    return "created";
  } else if (state.isDeleted) {
    return "deleted";
  } else if (state.oldKey !== state.newKey || state.oldValue !== state.newValue) {
    return "updated";
  }
  return undefined;
}

export function hasCustomAttributeChanged(state: CustomAttributeState): boolean {
  return getCustomAttributeStatus(state) !== undefined;
}

export function toCustomAttribute(customAttribute: CustomAttributeState): CustomAttribute {
  return { key: customAttribute.newKey, value: customAttribute.isDeleted ? "" : customAttribute.newValue };
}

export function toCustomAttributeState(customAttribute: CustomAttribute): CustomAttributeState {
  return {
    oldKey: customAttribute.key,
    oldValue: customAttribute.value,
    newKey: customAttribute.key,
    newValue: customAttribute.value,
    isDeleted: false,
  };
}

// TODO(fpion): unit tests
