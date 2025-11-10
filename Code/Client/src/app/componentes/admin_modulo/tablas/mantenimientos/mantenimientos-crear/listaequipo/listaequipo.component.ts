import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Equipos } from '../../../../../../models/admin/Equipos';
import { EquipoService } from '../../../../../../services/APIS/Equipo/equipo.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MantenimientosServiceEquipos } from '../../../../../../services/mantenimientoEquipos/mantenimientosEquipos.service';
import { FormularioDatosComponent } from './formulario-datos/formulario-datos.component';
import { BaseTablaComponent } from '../../../base/base';
import { MostrarerrorComponent } from '../../../../../pantallas_avisos/mostrarerror/mostrarerror.component';

@Component({
  selector: 'app-listaequipo',
  imports: [CommonModule , FormsModule, FormularioDatosComponent , MostrarerrorComponent],
  templateUrl: './listaequipo.component.html',
  styleUrl: './listaequipo.component.css'
})
export class ListaequipoComponent extends BaseTablaComponent {

  @Input() agregarequipo : WritableSignal<boolean> = signal(true);

  equipos : any[] = [];
  equiposcopia: any[] = [];

  equipoSeleccionado:  Equipos= new Equipos(); 

  terminoBusqueda: string = '';
  agregarEquipoSeleccionado: WritableSignal<boolean> = signal(false);


  constructor(private equiposapi : EquipoService ,public  mantenimientoequipos : MantenimientosServiceEquipos){
    super();
  }; 


  // ----
  sortColumn: string = 'Nombre';

  sortDirection: 'asc' | 'desc' = 'asc';


  ngOnInit(){
    this.cargarEquipos();
  }

  agregarEquipo(equipo : Equipos) {
   this.equipoSeleccionado = equipo;
    this.agregarEquipoSeleccionado.set(true);

  }


  limpiarEquipoSeleccionado() {
    this.equipoSeleccionado = new Equipos();
  }


  cargarEquipos() {
 
    this.equiposapi.obtenerEquipos().subscribe({
      next: (data: any[]) => {
        this.equipos = data;
        this.equiposcopia = [...this.equipos]; 
      },
      error: (error) => {
        this.mensajeerror = "Error al cargar los equipos, intente mas tarde";
        console.error('Error al cargar los equipos:', error);
        this.error.set(true);
      }
    });

  }

 // Función auxiliar para normalizar texto (remover acentos y convertir a minúsculas)
  private normalizeText(text: string): string {
    if (typeof text !== 'string') {
      return String(text || '').toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '');
    }
    return text
      .toLowerCase()
      .normalize('NFD')  // Descompone caracteres con acentos
      .replace(/[\u0300-\u036f]/g, '');  // Elimina diacríticos (acentos, tildes, etc.)
  }


  buscar(){
    if(this.terminoBusqueda.trim() === '') {
      this.limpiarBusqueda(); 
      return ; 
    }

    const busquedaNormalizada = this.normalizeText(this.terminoBusqueda);

    this.equipos = this.equiposcopia.filter(equipo =>
      this.normalizeText(equipo.Nombre).includes(busquedaNormalizada) ||
      this.normalizeText(equipo.Modelo).includes(busquedaNormalizada) ||
      this.normalizeText(equipo.Marca).includes(busquedaNormalizada) ||
      this.normalizeText(String(equipo.CodigoImt || '')).includes(busquedaNormalizada) ||
      this.normalizeText(equipo.CodigoUcb).includes(busquedaNormalizada) ||
      this.normalizeText(equipo.NumeroSerial).includes(busquedaNormalizada) ||
      this.normalizeText(equipo.NombreGrupoEquipo).includes(busquedaNormalizada)
    );
    
  }

  limpiarBusqueda(){
    this.terminoBusqueda = '';
    this.equipos = [...this.equiposcopia]; 
    
  }



// nose que hace
aplicarOrdenamiento() {
  this.equipos.sort((a, b) => {
    // Type assertion para acceso dinámico
    const valorA = (a as any)[this.sortColumn];
    const valorB = (b as any)[this.sortColumn];

    // Convertir a minúsculas si son strings
    let compA = typeof valorA === 'string' ? valorA.toLowerCase() : valorA;
    let compB = typeof valorB === 'string' ? valorB.toLowerCase() : valorB;

    if (compA < compB) {
      return this.sortDirection === 'asc' ? -1 : 1;
    } else if (compA > compB) {
      return this.sortDirection === 'asc' ? 1 : -1;
    } else {
      return 0;
    }
  });
}



ordenarPor(columna: string) {
 if (this.sortColumn === columna) {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
  } else {
    this.sortColumn = columna;
    this.sortDirection = 'asc';
  }

  this.aplicarOrdenamiento();    // Aplicar el ordenamiento

}


}