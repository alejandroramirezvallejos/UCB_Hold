import { TestBed } from '@angular/core/testing';

import { MantenimientoService } from './mantenimiento.service';

describe('MantenimientoService', () => {
  let service: MantenimientoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MantenimientoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
