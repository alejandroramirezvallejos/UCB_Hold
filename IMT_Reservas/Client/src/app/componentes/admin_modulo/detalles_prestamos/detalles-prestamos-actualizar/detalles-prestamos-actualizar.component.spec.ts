import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetallesPrestamosActualizarComponent } from './detalles-prestamos-actualizar.component';

describe('DetallesPrestamosActualizarComponent', () => {
  let component: DetallesPrestamosActualizarComponent;
  let fixture: ComponentFixture<DetallesPrestamosActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetallesPrestamosActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetallesPrestamosActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
