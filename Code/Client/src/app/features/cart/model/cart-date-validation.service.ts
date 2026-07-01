import { Injectable } from '@angular/core';
import { CartDateValidationResult } from './cart-date-validation-result';

@Injectable({
  providedIn: 'root',
})
export class CartDateValidationService {
  validate(
    startDate: Date | null,
    endDate: Date | null,
    currentDate: Date,
  ): CartDateValidationResult {
    if (!startDate || !endDate) {
      return { isValid: false, message: null };
    }

    const maximumStartDate = new Date(currentDate);
    maximumStartDate.setFullYear(currentDate.getFullYear() + 1);

    if (startDate > endDate) {
      return {
        isValid: false,
        message:
          'Error: La fecha de inicio no puede ser mayor a la fecha final',
      };
    }

    if (startDate < currentDate) {
      return {
        isValid: false,
        message:
          'Error: La fecha de inicio no puede ser menor a la fecha actual',
      };
    }

    if (maximumStartDate < startDate) {
      return {
        isValid: false,
        message:
          'Error: La fecha de inicio no puede ser mayor a un año desde la fecha actual',
      };
    }

    return { isValid: true, message: null };
  }
}
