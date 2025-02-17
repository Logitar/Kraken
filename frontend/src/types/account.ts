export type CurrentUser = {
  id: string;
  sessionId: string;
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

export type SignInPayload = {
  username: string;
  password: string;
};
