import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Mantenimientos } from '../../../../../models/admin/Mantenimientos';
import { MantenimientoService } from '../../../../../services/APIS/Mantenimiento/mantenimiento.service';
import { ListaequipoComponent } from './listaequipo/listaequipo.component';
import { MantenimientosServiceEquipos } from '../../../../../services/mantenimientoEquipos/mantenimientosEquipos.service';
import { CommonModule } from '@angular/common';
import { EmpresamantenimientoService } from '../../../../../services/APIS/EmpresaMantenimiento/empresamantenimiento.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-mantenimientos-crear',
  standalone: true,
  imports: [FormsModule , ListaequipoComponent, CommonModule, MostrarerrorComponent , Aviso , AvisoExitoComponent],
  templateUrl: './mantenimientos-crear.component.html',
  styleUrl: './mantenimientos-crear.component.css'
})
export class MantenimientosCrearComponent extends BaseTablaComponent{

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  agregarequipo : WritableSignal<boolean> = signal(false);

  fechaminima = this.toLocalISOString(new Date());
  

  mantenimiento: Mantenimientos = new Mantenimientos();
  empresas : string[] = [];

  mantenimientoSeleccionado: Map<number, {
        TipoMantenimiento: string;
        DescripcionEquipo : string ; 
        nombre : string }> = new Map();

  constructor(private mantenimientoapi: MantenimientoService , private mantenimientoequipo : MantenimientosServiceEquipos  , private empresa : EmpresamantenimientoService) {
    super();
   }


  
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
        this.mensajeerror = "Error al cargar las empresas de mantenimiento. Por favor, inténtelo de nuevo más tarde.";
        console.error('Error al obtener las empresas de mantenimiento:', error);
        this.error.set(true);
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

  validarcreacion(){
    this.mensajeaviso="¿Está seguro de que desea crear este mantenimiento?";
    this.aviso.set(true);
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

    this.mantenimientoapi.crearMantenimiento(mantenimientoParaEnvio , this.mantenimientoSeleccionado).subscribe({
      next: (response) => {
        this.Actualizar.emit();
        this.mensajeexito="Mantenimiento creado con éxito.";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror="Error al crear el mantenimiento. Por favor, inténtelo de nuevo más tarde.";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
  });
  }

  cerrar() {
    this.botoncrear.set(false);
  }

   toLocalISOString(date: Date): string {
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60000);
    return localDate.toISOString().split('T')[0];
  }


}
