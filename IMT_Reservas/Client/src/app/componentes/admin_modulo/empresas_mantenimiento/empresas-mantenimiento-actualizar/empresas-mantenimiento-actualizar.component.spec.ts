import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpresasMantenimientoActualizarComponent } from './empresas-mantenimiento-actualizar.component';

describe('EmpresasMantenimientoActualizarComponent', () => {
  let component: EmpresasMantenimientoActualizarComponent;
  let fixture: ComponentFixture<EmpresasMantenimientoActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmpresasMantenimientoActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmpresasMantenimientoActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
