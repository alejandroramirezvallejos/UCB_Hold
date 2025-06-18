import { TestBed } from '@angular/core/testing';

import { AccesoriosService } from './accesorios.service';

describe('AccesoriosService', () => {
  let service: AccesoriosService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AccesoriosService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
