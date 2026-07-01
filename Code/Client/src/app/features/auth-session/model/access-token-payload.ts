export interface AccessTokenPayload {
  exp?: number;
  role?: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
}
