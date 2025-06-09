import { TestBed } from '@angular/core/testing';

import { MandarPrestamoService } from './mandar-prestamo.service';

describe('MandarPrestamoService', () => {
  let service: MandarPrestamoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MandarPrestamoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
