import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpresasMantenimientoCrearComponent } from './empresas-mantenimiento-crear.component';

describe('EmpresasMantenimientoCrearComponent', () => {
  let component: EmpresasMantenimientoCrearComponent;
  let fixture: ComponentFixture<EmpresasMantenimientoCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmpresasMantenimientoCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmpresasMantenimientoCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
