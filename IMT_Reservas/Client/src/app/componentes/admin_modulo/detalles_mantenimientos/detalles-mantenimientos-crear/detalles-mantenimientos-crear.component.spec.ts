import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetallesMantenimientosCrearComponent } from './detalles-mantenimientos-crear.component';

describe('DetallesMantenimientosCrearComponent', () => {
  let component: DetallesMantenimientosCrearComponent;
  let fixture: ComponentFixture<DetallesMantenimientosCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetallesMantenimientosCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetallesMantenimientosCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
