import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetallesPrestamosEliminarComponent } from './detalles-prestamos-eliminar.component';

describe('DetallesPrestamosEliminarComponent', () => {
  let component: DetallesPrestamosEliminarComponent;
  let fixture: ComponentFixture<DetallesPrestamosEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetallesPrestamosEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetallesPrestamosEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
