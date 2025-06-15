import { TestBed } from '@angular/core/testing';

import { GrupoequipoService } from './grupoequipo.service';

describe('GrupoequipoService', () => {
  let service: GrupoequipoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GrupoequipoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
