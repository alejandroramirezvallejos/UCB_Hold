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
import { BuscadorComponent } from '../../../buscador/buscador.component';
import { Tabla } from '../../base/tabla';



@Component({
  selector: 'app-equipos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule , EquiposCrearComponent , EquiposEditarComponent,AvisoEliminarComponent , MostrarerrorComponent, AvisoExitoComponent , BuscadorComponent],
  templateUrl: './equipos-tabla.component.html',
  styleUrls: ['./equipos-tabla.component.css']
})
export class EquiposTablaComponent extends Tabla{

  botoncrear : WritableSignal<boolean> = signal(false);
  botoneditar : WritableSignal<boolean> = signal(false);
  alertaeliminar : boolean = false;
  equipos : any[] = [];
  equiposcopia: any[] = [];

  equipoSeleccionado:  Equipos= new Equipos();

  override columnas: string[] = ['Nombre','EstadoEquipo','Ubicacion','Código IMT','Costo'];



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

  
  aplicarFiltros(event?: [string, string]){
    if (event && event[0].trim() !== '') {
        const busquedaNormalizada = this.normalizeText(event[0]);
        this.equipos = this.equiposcopia.filter(equipo => {
          switch (event[1]) {
            case 'Nombre':
              return this.normalizeText(equipo.NombreGrupoEquipo || '').includes(busquedaNormalizada);
            case 'EstadoEquipo':
              return this.normalizeText(equipo.EstadoEquipo || '').includes(busquedaNormalizada);
            case 'Ubicacion':
              return this.normalizeText(equipo.Ubicacion || '').includes(busquedaNormalizada);
            case 'Código IMT':
              return this.normalizeText(String(equipo.CodigoImt || '')).includes(busquedaNormalizada);
            case 'Costo':
              return this.normalizeText(String(equipo.CostoReferencia || '')).includes(busquedaNormalizada);
            default:  // 'Todas las columnas'
              return this.normalizeText(equipo.NombreGrupoEquipo || '').includes(busquedaNormalizada) ||
                    this.normalizeText(equipo.EstadoEquipo || '').includes(busquedaNormalizada) ||
                    this.normalizeText(equipo.Ubicacion || '').includes(busquedaNormalizada) ||
                    this.normalizeText(String(equipo.CodigoImt || '')).includes(busquedaNormalizada) ||
                    this.normalizeText(String(equipo.CostoReferencia || '')).includes(busquedaNormalizada);
          }
        });
      } else {
        // Crear una copia para evitar referencias
        this.equipos = [...this.equiposcopia];
      }
  }

limpiarBusqueda(){
  
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
