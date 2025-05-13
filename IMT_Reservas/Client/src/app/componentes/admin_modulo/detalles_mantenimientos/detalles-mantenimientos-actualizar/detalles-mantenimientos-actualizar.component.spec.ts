import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetallesMantenimientosActualizarComponent } from './detalles-mantenimientos-actualizar.component';

describe('DetallesMantenimientosActualizarComponent', () => {
  let component: DetallesMantenimientosActualizarComponent;
  let fixture: ComponentFixture<DetallesMantenimientosActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetallesMantenimientosActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetallesMantenimientosActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
