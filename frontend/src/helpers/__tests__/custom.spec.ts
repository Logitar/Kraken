import { describe, it, expect } from "vitest";

import { getCustomAttributeStatus, hasCustomAttributeChanged, toCustomAttribute, toCustomAttributeState } from "../custom";
import type { CustomAttribute, CustomAttributeState } from "@/types/custom";

describe("getCustomAttributeStatus", () => {
  it.concurrent("should return the correct custom attribute status (created)", () => {
    const state: CustomAttributeState = {
      oldKey: "",
      oldValue: "",
      newKey: "key",
      newValue: "value",
      isDeleted: false,
    };
    expect(getCustomAttributeStatus(state)).toBe("created");
  });

  it.concurrent("should return the correct custom attribute status (deleted)", () => {
    const state: CustomAttributeState = {
      oldKey: "old-key",
      oldValue: "old-value",
      newKey: "new-key",
      newValue: "new-value",
      isDeleted: true,
    };
    expect(getCustomAttributeStatus(state)).toBe("deleted");
  });

  it.concurrent("should return the correct custom attribute status (undefined)", () => {
    const state: CustomAttributeState = {
      oldKey: "key",
      oldValue: "value",
      newKey: "key",
      newValue: "value",
      isDeleted: false,
    };
    expect(getCustomAttributeStatus(state)).toBeUndefined();
  });

  it.concurrent("should return the correct custom attribute status (updated)", () => {
    const state: CustomAttributeState = {
      oldKey: "old-key",
      oldValue: "old-value",
      newKey: "new-key",
      newValue: "new-value",
      isDeleted: false,
    };
    expect(getCustomAttributeStatus(state)).toBe("updated");
  });
});

describe("hasCustomAttributeChanged", () => {
  it.concurrent("should return false when the custom attribute has not changed", () => {
    const state: CustomAttributeState = {
      oldKey: "key",
      oldValue: "value",
      newKey: "key",
      newValue: "value",
      isDeleted: false,
    };
    expect(hasCustomAttributeChanged(state)).toBe(false);
  });

  it.concurrent("should return true when the custom attribute has changed", () => {
    let state: CustomAttributeState = {
      oldKey: "",
      oldValue: "",
      newKey: "key",
      newValue: "value",
      isDeleted: false,
    };
    expect(hasCustomAttributeChanged(state)).toBe(true);
    state = {
      oldKey: "old-key",
      oldValue: "old-value",
      newKey: "new-key",
      newValue: "new-value",
      isDeleted: true,
    };
    expect(hasCustomAttributeChanged(state)).toBe(true);
    state = {
      oldKey: "old-key",
      oldValue: "old-value",
      newKey: "new-key",
      newValue: "new-value",
      isDeleted: false,
    };
    expect(hasCustomAttributeChanged(state)).toBe(true);
  });
});

describe("toCustomAttribute", () => {
  it.concurrent("should return the correct custom attribute", () => {
    const state: CustomAttributeState = {
      oldKey: "",
      oldValue: "",
      newKey: "key",
      newValue: "value",
      isDeleted: false,
    };
    const customAttribute: CustomAttribute = toCustomAttribute(state);
    expect(customAttribute.key).toBe(state.newKey);
    expect(customAttribute.value).toBe(state.newValue);
  });
  it.concurrent("should return the correct deleted custom attribute", () => {
    const state: CustomAttributeState = {
      oldKey: "",
      oldValue: "",
      newKey: "key",
      newValue: "value",
      isDeleted: true,
    };
    const customAttribute: CustomAttribute = toCustomAttribute(state);
    expect(customAttribute.key).toBe(state.newKey);
    expect(customAttribute.value).toBe("");
  });
});

describe("toCustomAttributeState", () => {
  it.concurrent("should return the correct state", () => {
    const customAttribute: CustomAttribute = {
      key: "key",
      value: "value",
    };
    const state: CustomAttributeState = toCustomAttributeState(customAttribute);
    expect(state.oldKey).toBe(customAttribute.key);
    expect(state.oldValue).toBe(customAttribute.value);
    expect(state.newKey).toBe(customAttribute.key);
    expect(state.newValue).toBe(customAttribute.value);
    expect(state.isDeleted).toBe(false);
  });
});
