import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { EmpresasMantenimientoEditarComponent } from './empresas-mantenimiento-editar.component';
describe('EmpresasMantenimientoEditarComponent', () => {
  let component: EmpresasMantenimientoEditarComponent;
  let fixture: ComponentFixture<EmpresasMantenimientoEditarComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [EmpresasMantenimientoEditarComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(EmpresasMantenimientoEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
