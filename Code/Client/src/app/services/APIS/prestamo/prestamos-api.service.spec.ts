import { TestBed } from '@angular/core/testing';

import { PrestamosAPIService } from './prestamos-api.service';

describe('PrestamosAPIService', () => {
  let service: PrestamosAPIService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PrestamosAPIService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
