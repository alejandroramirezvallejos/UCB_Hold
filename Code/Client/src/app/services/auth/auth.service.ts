import { Injectable } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { Usuario } from '../../models/usuario';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly ACCESS_KEY  = 'ucbhold_access';
  private readonly REFRESH_KEY = 'ucbhold_refresh';
  private readonly USER_KEY    = 'ucbhold_user';

  private readonly http: HttpClient;

  constructor(backend: HttpBackend) {
    this.http = new HttpClient(backend);
  }

  setSession(accessToken: string, refreshToken: string, usuario: Usuario): void {
    localStorage.setItem(this.ACCESS_KEY,  accessToken);
    localStorage.setItem(this.REFRESH_KEY, refreshToken);
    localStorage.setItem(this.USER_KEY,    JSON.stringify(usuario));
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_KEY);
  }

  getStoredUser(): Usuario | null {
    try {
      return JSON.parse(localStorage.getItem(this.USER_KEY) ?? '');
    } catch {
      return null;
    }
  }

  isLoggedIn(): boolean {
    const accessToken = this.getAccessToken();
    if (!accessToken) return false;
    try {
      const base64Payload  = accessToken.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
      const decodedPayload = JSON.parse(atob(base64Payload));
      return decodedPayload['exp'] * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  getRole(): string | null {
    const accessToken = this.getAccessToken();
    if (!accessToken) return null;
    try {
      const base64Payload  = accessToken.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
      const decodedPayload = JSON.parse(atob(base64Payload));
      return decodedPayload['role'] || decodedPayload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
    } catch {
      return null;
    }
  }

  isAdmin(): boolean {
    return this.getRole() === 'administrador';
  }

  refreshTokens(refreshToken: string): Observable<{ accessToken: string; refreshToken: string }> {
    return this.http
      .post<any>(`${environment.apiUrl}/api/Usuario/refresh`, { RefreshToken: refreshToken })
      .pipe(
        map(rawResponse => ({
          accessToken:  rawResponse.Value.AccessToken  as string,
          refreshToken: rawResponse.Value.RefreshToken as string
        })),
        tap(newTokens => {
          localStorage.setItem(this.ACCESS_KEY,  newTokens.accessToken);
          localStorage.setItem(this.REFRESH_KEY, newTokens.refreshToken);
        })
      );
  }

  clear(): void {
    localStorage.removeItem(this.ACCESS_KEY);
    localStorage.removeItem(this.REFRESH_KEY);
    localStorage.removeItem(this.USER_KEY);
  }
}
