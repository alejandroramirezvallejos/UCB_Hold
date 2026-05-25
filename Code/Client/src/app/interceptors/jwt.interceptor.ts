import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  private activeRefresh       = false;
  private pendingAccessToken$ = new BehaviorSubject<string | null>(null);

  constructor(private authService: AuthService, private router: Router) {}

  intercept(outgoingRequest: HttpRequest<unknown>, nextHandler: HttpHandler): Observable<HttpEvent<unknown>> {
    const isAuthEndpoint = outgoingRequest.url.includes('/api/Usuario/login') ||
                           outgoingRequest.url.includes('/api/Usuario/refresh');

    if (isAuthEndpoint) return nextHandler.handle(outgoingRequest);

    const accessToken       = this.authService.getAccessToken();
    const authorizedRequest = accessToken
      ? outgoingRequest.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
      : outgoingRequest;

    return nextHandler.handle(authorizedRequest).pipe(
      catchError(responseError => {
        if (responseError instanceof HttpErrorResponse && responseError.status === 401) {
          return this.handleExpiredToken(outgoingRequest, nextHandler, responseError);
        }
        return throwError(() => responseError);
      })
    );
  }

  private handleExpiredToken(
    failedRequest: HttpRequest<unknown>,
    nextHandler: HttpHandler,
    originalError: HttpErrorResponse
  ): Observable<HttpEvent<unknown>> {
    const storedRefreshToken = this.authService.getRefreshToken();

    if (!storedRefreshToken) {
      if (this.authService.getAccessToken()) {
        this.authService.clear();
        this.router.navigate(['/Iniciar-Sesion']);
      }
      return throwError(() => originalError);
    }

    if (!this.activeRefresh) {
      this.activeRefresh = true;
      this.pendingAccessToken$.next(null);

      return this.authService.refreshTokens(storedRefreshToken).pipe(
        switchMap(renewedTokens => {
          this.activeRefresh = false;
          this.pendingAccessToken$.next(renewedTokens.accessToken);
          const retryRequest = failedRequest.clone({
            setHeaders: { Authorization: `Bearer ${renewedTokens.accessToken}` }
          });
          return nextHandler.handle(retryRequest);
        }),
        catchError(refreshError => {
          this.activeRefresh = false;
          this.authService.clear();
          this.router.navigate(['/Iniciar-Sesion']);
          return throwError(() => refreshError);
        })
      );
    }

    return this.pendingAccessToken$.pipe(
      filter(newAccessToken => newAccessToken !== null),
      take(1),
      switchMap(newAccessToken => {
        const retryRequest = failedRequest.clone({
          setHeaders: { Authorization: `Bearer ${newAccessToken!}` }
        });
        return nextHandler.handle(retryRequest);
      })
    );
  }
}
