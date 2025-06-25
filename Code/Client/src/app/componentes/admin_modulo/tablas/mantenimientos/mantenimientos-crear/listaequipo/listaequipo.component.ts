import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Equipos } from '../../../../../../models/admin/Equipos';
import { EquipoService } from '../../../../../../services/APIS/Equipo/equipo.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MantenimientosServiceEquipos } from '../../../../../../services/mantenimientoEquipos/mantenimientosEquipos.service';
import { FormularioDatosComponent } from './formulario-datos/formulario-datos.component';

@Component({
  selector: 'app-listaequipo',
  imports: [CommonModule , FormsModule, FormularioDatosComponent],
  templateUrl: './listaequipo.component.html',
  styleUrl: './listaequipo.component.css'
})
export class ListaequipoComponent {

  @Input() agregarequipo : WritableSignal<boolean> = signal(true);

  equipos : any[] = [];
  equiposcopia: any[] = [];

  equipoSeleccionado:  Equipos= new Equipos(); 

  terminoBusqueda: string = '';
  agregarEquipoSeleccionado: WritableSignal<boolean> = signal(false);


  constructor(private equiposapi : EquipoService ,public  mantenimientoequipos : MantenimientosServiceEquipos){}; 


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
 
    this.equiposapi.obtenerEquipos().subscribe(
      (data: any[]) => {
        this.equipos = data;
        this.equiposcopia = [...this.equipos]; 
      },
      (error) => {
        console.error('Error al cargar los equipos:', error);
      }
    );

  }

buscar(){
  if(this.terminoBusqueda.trim() === '') {
    this.limpiarBusqueda(); 
    return ; 
  }

  this.equipos = this.equiposcopia.filter(equipo =>
    equipo.Nombre?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    equipo.Modelo?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    equipo.Marca?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    String(equipo.CodigoImt || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    equipo.CodigoUcb?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    equipo.NumeroSerial?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    equipo.NombreGrupoEquipo?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) 
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