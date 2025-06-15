import { TestBed } from '@angular/core/testing';

import { ComponenteService } from './componente.service';

describe('ComponenteService', () => {
  let service: ComponenteService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ComponenteService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
