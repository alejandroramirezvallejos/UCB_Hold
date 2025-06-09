import { TestBed } from '@angular/core/testing';

import { ObtenercarrerasService } from './obtenercarreras.service';

describe('ObtenercarrerasService', () => {
  let service: ObtenercarrerasService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ObtenercarrerasService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
