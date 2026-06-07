export function extractErrorMessage(error: any, defaultMessage: string = 'Error desconocido'): string {
  if (!error) return defaultMessage;

  // HttpErrorResponse de Angular: su .message/.error son detalles de red/HTTP, no del backend.
  // Si no logramos extraer el mensaje real del cuerpo, usamos un mensaje según el status (no el crudo de Angular).
  const esHttpErrorResponse = typeof error.status === 'number' && typeof error.statusText === 'string';

  // Si el error tiene el formato estructurado del backend (Response<T>)
  if (error.error) {
    if (error.error.Errors && Array.isArray(error.error.Errors) && error.error.Errors.length > 0) {
      return error.error.Errors[0];
    }
    if (error.error.errors && Array.isArray(error.error.errors) && error.error.errors.length > 0) {
      return error.error.errors[0];
    }
    if (error.error.ValidationErrors && Array.isArray(error.error.ValidationErrors) && error.error.ValidationErrors.length > 0) {
      return error.error.ValidationErrors[0]?.description || error.error.ValidationErrors[0];
    }
    if (error.error.validationErrors && Array.isArray(error.error.validationErrors) && error.error.validationErrors.length > 0) {
      return error.error.validationErrors[0]?.description || error.error.validationErrors[0];
    }
    if (error.error.mensaje) {
      return error.error.mensaje;
    }
    if (error.error.message) {
      return error.error.message;
    }
    if (error.error.error) {
      return error.error.error;
    }
  }

  // Si el objeto de error principal tiene la info
  if (error.Errors && Array.isArray(error.Errors) && error.Errors.length > 0) {
    return error.Errors[0];
  }
  if (error.errors && Array.isArray(error.errors) && error.errors.length > 0) {
    return error.errors[0];
  }
  if (typeof error.error === 'string' && error.error.trim()) {
    return error.error;
  }

  // .message/.mensaje solo son confiables si NO vienen de un HttpErrorResponse
  // (en ese caso .message es texto crudo de Angular, p.ej. "Http failure response for url...: 404 Not Found")
  if (!esHttpErrorResponse) {
    if (error.message) return error.message;
    if (error.mensaje) return error.mensaje;
    if (typeof error === 'string') return error;
  }

  // Mensaje según código HTTP cuando el backend no mandó detalle
  if (esHttpErrorResponse) {
    if (error.status === 0) return 'No se pudo conectar con el servidor. Verifica tu conexión.';
    if (error.status === 404) return 'No se encontró el recurso solicitado.';
    if (error.status === 401 || error.status === 403) return 'No tienes permisos para realizar esta acción.';
    if (error.status >= 500) return 'Error en el servidor. Intenta nuevamente más tarde.';
  }

  return defaultMessage;
}
