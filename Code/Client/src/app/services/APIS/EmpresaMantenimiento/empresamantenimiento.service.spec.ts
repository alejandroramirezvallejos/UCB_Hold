import { TestBed } from '@angular/core/testing';

import { EmpresamantenimientoService } from './empresamantenimiento.service';

describe('EmpresamantenimientoService', () => {
  let service: EmpresamantenimientoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EmpresamantenimientoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
