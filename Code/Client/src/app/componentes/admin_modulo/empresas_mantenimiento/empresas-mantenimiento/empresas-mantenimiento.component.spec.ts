import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpresasMantenimientoComponent } from './empresas-mantenimiento.component';

describe('EmpresasMantenimientoComponent', () => {
  let component: EmpresasMantenimientoComponent;
  let fixture: ComponentFixture<EmpresasMantenimientoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmpresasMantenimientoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmpresasMantenimientoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
