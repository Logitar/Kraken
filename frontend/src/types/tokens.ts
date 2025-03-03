import type { Email, EmailPayload } from "./users";

export type Claim = {
  name: string;
  value: string;
  type?: string;
};

export type CreateTokenPayload = {
  isConsumable: boolean;
  algorithm?: string;
  audience?: string;
  issuer?: string;
  lifetimeSeconds?: number;
  secret?: string;
  type?: string;
  subject?: string;
  email?: EmailPayload;
  claims: Claim[];
};

export type CreatedToken = {
  token: string;
};

export type ValidatedToken = {
  subject?: string
  email?: Email
  claims: Claim[]
}

export type ValidateTokenPayload = {
  token: string
  consume: boolean
  audience?: string;
  issuer?: string;
  secret?: string;
  type?: string;
}
