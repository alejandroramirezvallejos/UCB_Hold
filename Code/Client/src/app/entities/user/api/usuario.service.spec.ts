import { TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { UsuarioServiceAPI } from './usuario.service';

describe('UsuarioServiceAPI', () => {
  let service: UsuarioServiceAPI;

  beforeEach(() => {
    TestBed.configureTestingModule(withDefaultTestingProviders({}));
    service = TestBed.inject(UsuarioServiceAPI);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
