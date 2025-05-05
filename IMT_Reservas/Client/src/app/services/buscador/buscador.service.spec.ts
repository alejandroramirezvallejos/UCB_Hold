import { TestBed } from '@angular/core/testing';

import { BuscadorService } from './buscador.service';

describe('BuscadorService', () => {
  let service: BuscadorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BuscadorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
