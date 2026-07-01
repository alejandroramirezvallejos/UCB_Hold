import { TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { EmpresamantenimientoService } from './empresamantenimiento.service';
describe('EmpresamantenimientoService', () => {
  let service: EmpresamantenimientoService;
  beforeEach(() => {
    TestBed.configureTestingModule(withDefaultTestingProviders({}));
    service = TestBed.inject(EmpresamantenimientoService);
  });
  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
