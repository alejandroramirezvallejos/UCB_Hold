import { TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { CarritoService } from './carrito.service';
describe('CarritoService', () => {
  let service: CarritoService;
  beforeEach(() => {
    TestBed.configureTestingModule(withDefaultTestingProviders({}));
    service = TestBed.inject(CarritoService);
  });
  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
