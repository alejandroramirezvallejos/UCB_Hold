import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetallesMantenimientosEliminarComponent } from './detalles-mantenimientos-eliminar.component';

describe('DetallesMantenimientosEliminarComponent', () => {
  let component: DetallesMantenimientosEliminarComponent;
  let fixture: ComponentFixture<DetallesMantenimientosEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetallesMantenimientosEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetallesMantenimientosEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
