import { TestBed } from '@angular/core/testing';

import { MantenimientosServiceEquipos } from './mantenimientosEquipos.service';

describe('MantenimientosService', () => {
  let service: MantenimientosServiceEquipos;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MantenimientosServiceEquipos);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
