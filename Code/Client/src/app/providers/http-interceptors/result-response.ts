import { ValidationErrorResponse } from './validation-error-response';

export interface ResultResponse<T> {
  status: number;
  value: T;
  errors: string[];
  validationErrors: ValidationErrorResponse[];
  successMessage?: string;
}
