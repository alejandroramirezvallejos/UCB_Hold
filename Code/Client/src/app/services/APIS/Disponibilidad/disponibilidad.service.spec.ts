import { TestBed } from '@angular/core/testing';

import { DisponibilidadService } from './disponibilidad.service';

describe('DisponibilidadService', () => {
  let service: DisponibilidadService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DisponibilidadService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
