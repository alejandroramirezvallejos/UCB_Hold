import { TestBed } from '@angular/core/testing';

import { MandarcarritoService } from './mandarcarrito.service';

describe('MandarcarritoService', () => {
  let service: MandarcarritoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MandarcarritoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
