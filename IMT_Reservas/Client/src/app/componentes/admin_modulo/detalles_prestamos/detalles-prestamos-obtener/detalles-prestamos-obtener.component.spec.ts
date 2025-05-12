import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetallesPrestamosObtenerComponent } from './detalles-prestamos-obtener.component';

describe('DetallesPrestamosObtenerComponent', () => {
  let component: DetallesPrestamosObtenerComponent;
  let fixture: ComponentFixture<DetallesPrestamosObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetallesPrestamosObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetallesPrestamosObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
