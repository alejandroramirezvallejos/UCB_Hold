import { TestBed } from '@angular/core/testing';

import { MantenimientosService } from './mantenimientos.service';

describe('MantenimientosService', () => {
  let service: MantenimientosService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MantenimientosService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
