import { TestBed } from '@angular/core/testing';

import { GrupoEquipoService } from './grupo-equipo.service';

describe('GrupoEquipoService', () => {
  let service: GrupoEquipoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GrupoEquipoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
