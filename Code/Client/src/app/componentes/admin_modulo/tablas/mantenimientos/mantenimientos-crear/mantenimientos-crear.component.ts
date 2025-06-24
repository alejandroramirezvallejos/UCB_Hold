import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Mantenimientos } from '../../../../../models/admin/Mantenimientos';
import { MantenimientoService } from '../../../../../services/APIS/Mantenimiento/mantenimiento.service';
import { ListaequipoComponent } from './listaequipo/listaequipo.component';
import { MantenimientosServiceEquipos } from '../../../../../services/mantenimientoEquipos/mantenimientosEquipos.service';
import { CommonModule } from '@angular/common';
import { EmpresamantenimientoService } from '../../../../../services/APIS/EmpresaMantenimiento/empresamantenimiento.service';

@Component({
  selector: 'app-mantenimientos-crear',
  standalone: true,
  imports: [FormsModule , ListaequipoComponent, CommonModule],
  templateUrl: './mantenimientos-crear.component.html',
  styleUrl: './mantenimientos-crear.component.css'
})
export class MantenimientosCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  agregarequipo : WritableSignal<boolean> = signal(false);

  fechaminima = new Date().toISOString().split('T')[0];

  

  mantenimiento: Mantenimientos = {
    Id: 0,
    NombreEmpresaMantenimiento: '',
    FechaMantenimiento: null,
    FechaFinalDeMantenimiento: null,
    Costo: 0,
    Descripcion: '',
    TipoMantenimiento: '',
    NombreGrupoEquipo: '',
    CodigoImtEquipo: 0,
    DescripcionEquipo: ''
  };
  empresas : string[] = [];

  mantenimientoSeleccionado: Map<number, {
        TipoMantenimiento: string;
        DescripcionEquipo : string ; 
        nombre : string }> = new Map();

  constructor(private mantenimientoapi: MantenimientoService , private mantenimientoequipo : MantenimientosServiceEquipos  , private empresa : EmpresamantenimientoService) { }


  
  ngOnInit() {
    this.obtenermantenimientoSeleccionado();
    this.obtenereempresasMantenimiento();
  }

  ngOnDestroy() {
    this.mantenimientoequipo.vaciarEquiposMantenimientos(); 
  }

  validarFecha() {
    if (!this.mantenimiento.FechaMantenimiento || !this.mantenimiento.FechaFinalDeMantenimiento) {
    return false;
  }
  
 
  const fechaInicio = new Date(this.mantenimiento.FechaMantenimiento);
  const fechaFinal = new Date(this.mantenimiento.FechaFinalDeMantenimiento);
  const fechaMinima = new Date(this.fechaminima);
  
  if (fechaInicio > fechaFinal) {
    return false;
  }
  
  if (fechaInicio < fechaMinima) {
    return false;
  }
  
  return true;
  }

  fechamaxima(fecha : Date | null){
    if (!fecha) {
      return null;
    }
      const fechaMaxima = new Date(fecha);
      fechaMaxima.setFullYear(fechaMaxima.getFullYear() + 1);
      return fechaMaxima;
    

  }


  obtenereempresasMantenimiento() {
    this.empresa.obtenerEmpresaMantenimiento().subscribe({
      next: (empresas) => {
        this.empresas = empresas.map(empresa => empresa.NombreEmpresa);

      },
      error: (error) => {
        console.error('Error al obtener las empresas de mantenimiento:', error);
        alert('Error al cargar las empresas de mantenimiento. Por favor, inténtelo de nuevo más tarde.');
      }
    }
    );
  }

  agregarEquipo() {
    this.agregarequipo.set(!this.agregarequipo());
  }

  obtenermantenimientoSeleccionado() {
    this.mantenimientoSeleccionado= this.mantenimientoequipo.obtenerEquiposMantenimientos();
  }

  eliminarEquipo(codigo: number) {
    this.mantenimientoequipo.quitarequipo(codigo);
    this.obtenermantenimientoSeleccionado();
  }

  registrar() {
    // Convertir el objeto para que coincida con lo que espera el servicio
    const mantenimientoParaEnvio = {
      FechaMantenimiento: this.mantenimiento.FechaMantenimiento,
      FechaFinalDeMantenimiento: this.mantenimiento.FechaFinalDeMantenimiento,
      NombreEmpresaMantenimiento: this.mantenimiento.NombreEmpresaMantenimiento,
      Costo: this.mantenimiento.Costo,
      DescripcionMantenimiento: this.mantenimiento.Descripcion,
    };

    this.mantenimientoapi.crearMantenimiento(mantenimientoParaEnvio , this.mantenimientoSeleccionado).subscribe(
      response => {
        this.Actualizar.emit();
        this.cerrar();
      },
      error => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    );
  }

  cerrar() {
    this.botoncrear.set(false);
  }
}
