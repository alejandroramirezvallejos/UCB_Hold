import { TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { PrestamosAPIService } from './prestamos-api.service';
describe('PrestamosAPIService', () => {
  let service: PrestamosAPIService;
  beforeEach(() => {
    TestBed.configureTestingModule(withDefaultTestingProviders({}));
    service = TestBed.inject(PrestamosAPIService);
  });
  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
