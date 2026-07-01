import { TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { GrupoequipoService } from './grupo-equipo.service';
describe('GrupoequipoService', () => {
  let service: GrupoequipoService;
  beforeEach(() => {
    TestBed.configureTestingModule(withDefaultTestingProviders({}));
    service = TestBed.inject(GrupoequipoService);
  });
  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
