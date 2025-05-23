import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpresasMantenimientoObtenerComponent } from './empresas-mantenimiento-obtener.component';

describe('EmpresasMantenimientoObtenerComponent', () => {
  let component: EmpresasMantenimientoObtenerComponent;
  let fixture: ComponentFixture<EmpresasMantenimientoObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmpresasMantenimientoObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmpresasMantenimientoObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
