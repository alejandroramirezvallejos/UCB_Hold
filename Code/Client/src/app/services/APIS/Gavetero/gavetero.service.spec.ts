import { TestBed } from '@angular/core/testing';

import { GaveteroService } from './gavetero.service';

describe('GaveteroService', () => {
  let service: GaveteroService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GaveteroService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
