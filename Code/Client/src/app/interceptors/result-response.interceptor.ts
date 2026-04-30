import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

interface ResultResponse<T> {
  status: number;
  value: T;
  errors: string[];
  validationErrors: Array<{ propertyName: string; description: string }>;
  successMessage?: string;
}

@Injectable()
export class ResultResponseInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      map(event => {
        if (event instanceof HttpResponse) {
          const body = event.body as ResultResponse<any>;

          if (this.isResultResponse(body)) {
            switch (body.status) {
              case 200:
              case 201:
                return event.clone({ body: { message: body.successMessage || 'Operación exitosa', data: body.value } });

              case 400:
                const validationErrors = body.validationErrors?.map(e => `${e.propertyName}: ${e.description}`).join(', ') || 'Errores de validación';
                const errorResponse = {
                  message: validationErrors,
                  errors: body.validationErrors || [],
                  statusCode: 400
                };
                return event.clone({ body: errorResponse });

              case 404:
                const notFoundError = {
                  message: body.errors?.[0] || 'Recurso no encontrado',
                  statusCode: 404
                };
                return event.clone({ body: notFoundError });

              case 409:
                const conflictError = {
                  message: body.errors?.[0] || 'Conflicto en la solicitud',
                  statusCode: 409
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
        const body = error.error as ResultResponse<any>;

        if (this.isResultResponse(body)) {
          const errorData = {
            message: body.errors?.[0] || body.successMessage || 'Error en la solicitud',
            errors: body.validationErrors || [],
            statusCode: error.status
          };
          return throwError(() => ({
            status: error.status,
            error: errorData
          }));
        }

        return throwError(() => error);
      })
    );
  }

  private isResultResponse(body: any): boolean {
    return body &&
           typeof body === 'object' &&
           'status' in body &&
           'value' in body &&
           'errors' in body;
  }
}
