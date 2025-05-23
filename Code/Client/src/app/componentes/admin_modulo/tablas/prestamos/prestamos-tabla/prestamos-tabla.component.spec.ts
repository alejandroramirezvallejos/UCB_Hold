import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrestamosTablaComponent } from './prestamos-tabla.component';

describe('PrestamosTablaComponent', () => {
  let component: PrestamosTablaComponent;
  let fixture: ComponentFixture<PrestamosTablaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrestamosTablaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrestamosTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
