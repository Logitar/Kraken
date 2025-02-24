export type CustomAttribute = {
  key: string;
  value: string;
};

export type CustomAttributeState = {
  oldKey: string;
  oldValue: string;
  newKey: string;
  newValue: string;
  isDeleted: boolean;
};

export type CustomAttributeStatus = "created" | "deleted" | "updated";
