import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Equipos } from '../../../../../models/admin/Equipos';
import { EquiposCrearComponent } from '../equipos-crear/equipos-crear.component';
import { EquiposEditarComponent } from '../equipos-editar/equipos-editar.component';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';



@Component({
  selector: 'app-equipos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule , EquiposCrearComponent , EquiposEditarComponent,AvisoEliminarComponent , MostrarerrorComponent, AvisoExitoComponent],
  templateUrl: './equipos-tabla.component.html',
  styleUrls: ['./equipos-tabla.component.css']
})
export class EquiposTablaComponent extends BaseTablaComponent{

  botoncrear : WritableSignal<boolean> = signal(false);
  botoneditar : WritableSignal<boolean> = signal(false);
  alertaeliminar : boolean = false;
  equipos : any[] = [];
  equiposcopia: any[] = [];

  equipoSeleccionado:  Equipos= new Equipos();

  terminoBusqueda: string = '';


  constructor(private equiposapi : EquipoService){
    super(); 
  }; 



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
        this.mensajeerror = "Error al cargar los equipos";
        console.error('Error al cargar los equipos:', error);
        this.error.set(true);
      }
    );

  }
private normalizeText(text: string): string {
    return text
      .toLowerCase()
      .normalize('NFD')  // Descompone caracteres con acentos
      .replace(/[\u0300-\u036f]/g, '');  // Elimina diacríticos
  }
  
buscar(){
  if(this.terminoBusqueda.trim() === '') {
    this.limpiarBusqueda();
    return;
  }

  const busquedaNormalizada = this.normalizeText(this.terminoBusqueda);

  this.equipos = this.equiposcopia.filter(equipo =>
    this.normalizeText(equipo.Nombre || '').includes(busquedaNormalizada) ||
    this.normalizeText(equipo.Modelo || '').includes(busquedaNormalizada) ||
    this.normalizeText(equipo.Marca || '').includes(busquedaNormalizada) ||
    String(this.normalizeText(equipo.CodigoImt || '')).includes(busquedaNormalizada) ||
    this.normalizeText(equipo.CodigoUcb || '').includes(busquedaNormalizada) ||
    this.normalizeText(equipo.NumeroSerial || '').includes(busquedaNormalizada) ||
    this.normalizeText(equipo.NombreGrupoEquipo || '').includes(busquedaNormalizada)
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
  this.equiposapi.eliminarEquipo(this.equipoSeleccionado.Id).subscribe({
    next:(response) => {
      this.mensajeexito = "Equipo eliminado con éxito";
      this.exito.set(true);
       this.cargarEquipos(); 

    },
    error: (error) => {
      this.mensajeerror = "Error al eliminar el equipo";
      console.error('Error al eliminar el equipo: ' + error);
      this.error.set(true);
    }}
  );
  this.limpiarEquipoSeleccionado();
  this.alertaeliminar = false;
}

cancelarEliminacion(){
  this.alertaeliminar = false; 
  this.limpiarEquipoSeleccionado(); 
}








}
