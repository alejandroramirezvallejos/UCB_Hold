import { UsuarioApiItem } from './usuario-api-item';

export interface UsuarioLoginApiValue {
  AccessToken: string;
  RefreshToken: string;
  Usuario: UsuarioApiItem;
}
