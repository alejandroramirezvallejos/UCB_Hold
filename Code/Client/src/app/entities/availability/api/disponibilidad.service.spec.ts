import { TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { DisponibilidadService } from './disponibilidad.service';
describe('DisponibilidadService', () => {
  let service: DisponibilidadService;
  beforeEach(() => {
    TestBed.configureTestingModule(withDefaultTestingProviders({}));
    service = TestBed.inject(DisponibilidadService);
  });
  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
