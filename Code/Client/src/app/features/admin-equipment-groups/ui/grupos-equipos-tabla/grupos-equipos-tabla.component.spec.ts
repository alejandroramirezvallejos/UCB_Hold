import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { GrupoEquipo } from '@entities/equipment-group';
import { GruposEquiposTablaComponent } from './grupos-equipos-tabla.component';

describe('GruposEquiposTablaComponent', () => {
  let component: GruposEquiposTablaComponent;
  let fixture: ComponentFixture<GruposEquiposTablaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [GruposEquiposTablaComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(GruposEquiposTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should sort groups by name in both directions', () => {
    component.gruposEquiposFiltrados = [
      Object.assign(new GrupoEquipo(), { id: 1, nombre: 'Zeta' }),
      Object.assign(new GrupoEquipo(), { id: 2, nombre: 'Alpha' }),
    ];

    component.ordenarPorColumna('Nombre');

    expect(
      component.gruposEquiposFiltrados.map((grupo) => grupo.nombre),
    ).toEqual(['Alpha', 'Zeta']);

    component.ordenarPorColumna('Nombre');

    expect(
      component.gruposEquiposFiltrados.map((grupo) => grupo.nombre),
    ).toEqual(['Zeta', 'Alpha']);
  });

  it('should reorder the rendered rows when the header icon is clicked', () => {
    component.gruposEquiposFiltrados = [
      Object.assign(new GrupoEquipo(), { id: 1, nombre: 'Zeta' }),
      Object.assign(new GrupoEquipo(), { id: 2, nombre: 'Alpha' }),
    ];
    fixture.detectChanges();

    const filasNombre = (): string[] =>
      Array.from(
        fixture.nativeElement.querySelectorAll('tbody tr td:first-child'),
      ).map((celda) => (celda as HTMLElement).textContent!.trim());

    expect(filasNombre()).toEqual(['Zeta', 'Alpha']);

    const icono: HTMLElement = fixture.nativeElement.querySelector(
      '.sortable-th .sort-icon',
    );
    icono.click();
    fixture.detectChanges();

    expect(filasNombre()).toEqual(['Alpha', 'Zeta']);
  });
});
