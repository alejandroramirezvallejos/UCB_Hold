import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { PrestamoDto } from '@entities/admin';
import { PrestamosTablaComponent } from './prestamos-tabla.component';

describe('PrestamosTablaComponent', () => {
  let component: PrestamosTablaComponent;
  let fixture: ComponentFixture<PrestamosTablaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [PrestamosTablaComponent],
      }),
    ).compileComponents();

    fixture = TestBed.createComponent(PrestamosTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initially render newest loans first by Fecha Solicitud', () => {
    cargarPrestamos([
      crearPrestamo({
        id: 1,
        nombre: 'Antiguo',
        fechaSolicitud: '2026-06-12T18:21:00',
      }),
      crearPrestamo({
        id: 2,
        nombre: 'Reciente',
        fechaSolicitud: '2026-06-14T12:38:00',
      }),
    ]);

    expect(nombresRenderizados()).toEqual(['Reciente', 'Antiguo']);
  });

  it('should use the global sorter when a loan table header is clicked', () => {
    cargarPrestamos([
      crearPrestamo({ id: 1, nombre: 'CarnetMayor', carnet: '200' }),
      crearPrestamo({ id: 2, nombre: 'CarnetMenor', carnet: '100' }),
    ]);

    botonCabecera('Carnet').click();
    fixture.detectChanges();

    expect(nombresRenderizados()).toEqual(['CarnetMenor', 'CarnetMayor']);

    botonCabecera('Carnet').click();
    fixture.detectChanges();

    expect(nombresRenderizados()).toEqual(['CarnetMayor', 'CarnetMenor']);
  });

  function cargarPrestamos(prestamos: PrestamoDto[]): void {
    component.sortColumn = 'Fecha Solicitud';
    component.sortDirection = 'desc';
    component.agruparPrestamos(prestamos);
    component.aplicarFiltros();
    fixture.detectChanges();
  }

  function crearPrestamo(datos: {
    id: number;
    nombre: string;
    carnet?: string;
    telefono?: string;
    equipo?: string;
    fechaSolicitud?: string;
    fechaPrestamoEsperada?: string;
    fechaDevolucionEsperada?: string;
    estado?: string;
  }): PrestamoDto {
    return Object.assign(new PrestamoDto(), {
      Id: datos.id,
      NombreUsuario: datos.nombre,
      ApellidoPaternoUsuario: '',
      CarnetUsuario: datos.carnet ?? '12890061',
      TelefonoUsuario: datos.telefono ?? '799430792',
      NombreGrupoEquipo: datos.equipo ?? 'Mini Dron',
      FechaSolicitud: new Date(datos.fechaSolicitud ?? '2026-06-12T18:21:00'),
      FechaPrestamoEsperada: new Date(
        datos.fechaPrestamoEsperada ?? '2026-06-13T00:00:00',
      ),
      FechaDevolucionEsperada: new Date(
        datos.fechaDevolucionEsperada ?? '2026-06-14T00:00:00',
      ),
      EstadoPrestamo: datos.estado ?? 'pendiente',
    });
  }

  function botonCabecera(nombre: string): HTMLElement {
    const botones = Array.from(
      fixture.nativeElement.querySelectorAll('.table-sort-button'),
    ) as HTMLElement[];
    const boton = botones.find((elemento) =>
      elemento.textContent?.includes(nombre),
    );

    if (!boton) throw new Error(`No se encontró la cabecera ${nombre}`);

    return boton;
  }

  function nombresRenderizados(): string[] {
    return Array.from(
      fixture.nativeElement.querySelectorAll('.nombre-usuario span'),
    ).map((elemento) => (elemento as HTMLElement).textContent!.trim());
  }
});
