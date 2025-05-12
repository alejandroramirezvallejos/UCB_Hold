import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetallesMantenimientosObtenerComponent } from './detalles-mantenimientos-obtener.component';

describe('DetallesMantenimientosObtenerComponent', () => {
  let component: DetallesMantenimientosObtenerComponent;
  let fixture: ComponentFixture<DetallesMantenimientosObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetallesMantenimientosObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetallesMantenimientosObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
