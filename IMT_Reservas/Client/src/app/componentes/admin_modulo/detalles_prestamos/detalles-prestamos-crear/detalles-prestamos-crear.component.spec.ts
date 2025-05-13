import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetallesPrestamosCrearComponent } from './detalles-prestamos-crear.component';

describe('DetallesPrestamosCrearComponent', () => {
  let component: DetallesPrestamosCrearComponent;
  let fixture: ComponentFixture<DetallesPrestamosCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetallesPrestamosCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetallesPrestamosCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
