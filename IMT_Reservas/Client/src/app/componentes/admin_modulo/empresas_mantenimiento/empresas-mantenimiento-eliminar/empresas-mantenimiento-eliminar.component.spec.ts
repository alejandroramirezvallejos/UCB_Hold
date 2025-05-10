import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpresasMantenimientoEliminarComponent } from './empresas-mantenimiento-eliminar.component';

describe('EmpresasMantenimientoEliminarComponent', () => {
  let component: EmpresasMantenimientoEliminarComponent;
  let fixture: ComponentFixture<EmpresasMantenimientoEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmpresasMantenimientoEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmpresasMantenimientoEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
