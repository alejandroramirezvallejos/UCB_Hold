import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrestamosComponent } from './prestamos.component';

describe('PrestamosComponent', () => {
  let component: PrestamosComponent;
  let fixture: ComponentFixture<PrestamosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrestamosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrestamosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
