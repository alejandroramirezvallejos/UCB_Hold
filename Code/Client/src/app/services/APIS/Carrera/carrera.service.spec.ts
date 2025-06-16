import { TestBed } from '@angular/core/testing';

import { CarreraService } from './carrera.service';

describe('CarreraService', () => {
  let service: CarreraService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CarreraService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
