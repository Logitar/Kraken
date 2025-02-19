export type CustomAttribute = {
  key: string;
  value: string;
};

export class CustomAttributeState {
  private oldKey: string;
  private oldValue: string;
  private newKey: string;
  private newValue: string;
  private isDeleted: boolean;

  public get status(): CustomAttributeStatus | undefined {
    if (!this.oldKey || !this.oldValue) {
      return "created";
    } else if (this.oldKey !== this.newKey || this.oldValue !== this.newValue) {
      return "updated";
    } else if (this.isDeleted) {
      return "deleted";
    }
  }
  public get hasChanges(): boolean {
    return this.status !== undefined;
  }

  constructor(customAttribute?: CustomAttribute) {
    this.oldKey = this.newKey = customAttribute?.key ?? "";
    this.oldValue = this.newValue = customAttribute?.value ?? "";
    this.isDeleted = false;
  }

  public delete(): void {
    this.isDeleted = true;
  }
  public restore(): void {
    this.isDeleted = false;
  }

  public toCustomAttribute(): CustomAttribute {
    return { key: this.newKey, value: this.newValue };
  }
}

export type CustomAttributeStatus = "created" | "deleted" | "updated";
