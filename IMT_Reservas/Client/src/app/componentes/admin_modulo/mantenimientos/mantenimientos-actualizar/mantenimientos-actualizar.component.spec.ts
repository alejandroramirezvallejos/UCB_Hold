import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MantenimientosActualizarComponent } from './mantenimientos-actualizar.component';

describe('MantenimientosActualizarComponent', () => {
  let component: MantenimientosActualizarComponent;
  let fixture: ComponentFixture<MantenimientosActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MantenimientosActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MantenimientosActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
