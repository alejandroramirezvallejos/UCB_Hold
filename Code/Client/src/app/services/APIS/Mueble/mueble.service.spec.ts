import { TestBed } from '@angular/core/testing';

import { MuebleService } from './mueble.service';

describe('MuebleService', () => {
  let service: MuebleService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MuebleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
