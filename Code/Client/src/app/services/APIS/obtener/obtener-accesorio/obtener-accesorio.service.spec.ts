import { TestBed } from '@angular/core/testing';

import { ObtenerAccesorioService } from './obtener-accesorio.service';

describe('ObtenerAccesorioService', () => {
  let service: ObtenerAccesorioService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ObtenerAccesorioService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
