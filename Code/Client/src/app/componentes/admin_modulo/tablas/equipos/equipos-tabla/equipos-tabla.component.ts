import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Equipos } from '../../../../../models/admin/Equipos';
import { EquiposCrearComponent } from '../equipos-crear/equipos-crear.component';
import { EquiposEditarComponent } from '../equipos-editar/equipos-editar.component';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';



@Component({
  selector: 'app-equipos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule , EquiposCrearComponent , EquiposEditarComponent,AvisoEliminarComponent],
  templateUrl: './equipos-tabla.component.html',
  styleUrls: ['./equipos-tabla.component.css']
})
export class EquiposTablaComponent {

  botoncrear : WritableSignal<boolean> = signal(false);
  botoneditar : WritableSignal<boolean> = signal(false);
  alertaeliminar : boolean = false;
  equipos : any[] = [];
  equiposcopia: any[] = [];

  equipoSeleccionado:  Equipos= new Equipos();

  terminoBusqueda: string = '';


  constructor(private equiposapi : EquipoService){}; 



  ngOnInit(){
    this.cargarEquipos();
  }


  limpiarEquipoSeleccionado() {
    this.equipoSeleccionado = new Equipos();
  }



  crearequipo() {
    this.botoncrear.set(true);
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
    (equipo.Nombre|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    (equipo.Modelo|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    (equipo.Marca|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    String(equipo.CodigoImt || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    (equipo.CodigoUcb|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    (equipo.NumeroSerial|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    (equipo.NombreGrupoEquipo|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) 
  );
}

limpiarBusqueda(){
  this.terminoBusqueda = '';
  this.equipos = [...this.equiposcopia]; 
  
}

editarEquipo(equipo : any) {
  this.botoncrear.set(false);
  this.equipoSeleccionado = { ...equipo }; // Crear una copia del objeto
  this.botoneditar.set(true);
}

eliminarEquipo(equipo : any) {
  this.equipoSeleccionado = equipo;
  this.alertaeliminar = true;
 
}

confirmarEliminacion() {
  this.equiposapi.eliminarEquipo(this.equipoSeleccionado.Id).subscribe(
    (response) => {

       this.cargarEquipos(); 

    },
    (error) => {
      alert('Error al eliminar el equipo: ' + error);
    }
  );
  this.limpiarEquipoSeleccionado();
  this.alertaeliminar = false;
}

cancelarEliminacion(){
  this.alertaeliminar = false; 
  this.limpiarEquipoSeleccionado(); 
}








}
