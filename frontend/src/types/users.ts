import type { Actor } from "./actor";

export type Contact = {
  isVerified: boolean;
  verifiedBy?: Actor
  verifiedOn?: string
}

export type ContactPayload = {
  isVerified: boolean;
};

export type Email = Contact & {
  address: string;
}

export type EmailPayload = ContactPayload & {
  address: string;
};
