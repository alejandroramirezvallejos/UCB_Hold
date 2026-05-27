export function extractErrorMessage(error: any, defaultMessage: string = 'Error desconocido'): string {
  if (!error) return defaultMessage;
  
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
  if (error.message) {
    return error.message;
  }
  if (error.mensaje) {
    return error.mensaje;
  }

  // Si es un string directo
  if (typeof error === 'string') {
    return error;
  }
  
  if (typeof error.error === 'string') {
    return error.error;
  }

  return defaultMessage;
}
