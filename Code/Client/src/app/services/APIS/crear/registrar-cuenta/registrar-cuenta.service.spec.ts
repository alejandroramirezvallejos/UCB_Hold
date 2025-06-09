import { TestBed } from '@angular/core/testing';

import { RegistrarCuentaService } from './registrar-cuenta.service';

describe('RegistrarCuentaService', () => {
  let service: RegistrarCuentaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RegistrarCuentaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
