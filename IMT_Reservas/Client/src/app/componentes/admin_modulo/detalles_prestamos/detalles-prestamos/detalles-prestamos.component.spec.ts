import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetallesPrestamosComponent } from './detalles-prestamos.component';

describe('DetallesPrestamosComponent', () => {
  let component: DetallesPrestamosComponent;
  let fixture: ComponentFixture<DetallesPrestamosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetallesPrestamosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetallesPrestamosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
