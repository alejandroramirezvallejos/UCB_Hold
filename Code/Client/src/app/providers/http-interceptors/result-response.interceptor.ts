import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ResultResponse } from './result-response';

@Injectable()
export class ResultResponseInterceptor implements HttpInterceptor {
  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler,
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      map((event) => {
        if (event instanceof HttpResponse) {
          const body = event.body as ResultResponse<unknown>;

          if (this.isResultResponse(body)) {
            switch (body.status) {
              case 200:
              case 201:
                return event.clone({ body: body.value });

              case 400:
                const validationErrors =
                  body.validationErrors
                    ?.map((e) => `${e.propertyName}: ${e.description}`)
                    .join(', ') || 'Errores de validación';
                const errorResponse = {
                  message: validationErrors,
                  errors: body.validationErrors || [],
                  statusCode: 400,
                };
                return event.clone({ body: errorResponse });

              case 404:
                const notFoundError = {
                  message: body.errors?.[0] || 'Recurso no encontrado',
                  statusCode: 404,
                };
                return event.clone({ body: notFoundError });

              case 409:
                const conflictError = {
                  message: body.errors?.[0] || 'Conflicto en la solicitud',
                  statusCode: 409,
                };
                return event.clone({ body: conflictError });

              default:
                return event;
            }
          }
        }
        return event;
      }),
      catchError((error: HttpErrorResponse) => {
        const body = error.error as ResultResponse<unknown>;

        if (this.isResultResponse(body)) {
          const errorData = {
            message:
              body.errors?.[0] ||
              body.successMessage ||
              'Error en la solicitud',
            errors: body.validationErrors || [],
            statusCode: error.status,
          };
          return throwError(() => ({
            status: error.status,
            error: errorData,
          }));
        }

        return throwError(() => error);
      }),
    );
  }

  private isResultResponse(body: unknown): body is ResultResponse<unknown> {
    return Boolean(
      body &&
      typeof body === 'object' &&
      'status' in body &&
      'value' in body &&
      'errors' in body,
    );
  }
}
