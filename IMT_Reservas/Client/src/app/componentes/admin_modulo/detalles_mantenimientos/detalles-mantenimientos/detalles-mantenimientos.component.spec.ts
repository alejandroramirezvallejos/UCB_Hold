import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetallesMantenimientosComponent } from './detalles-mantenimientos.component';

describe('DetallesMantenimientosComponent', () => {
  let component: DetallesMantenimientosComponent;
  let fixture: ComponentFixture<DetallesMantenimientosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetallesMantenimientosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetallesMantenimientosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
