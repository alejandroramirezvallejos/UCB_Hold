import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Gaveteros } from '../../../../../models/admin/Gaveteros';
import { GaveterosCrearComponent } from '../gaveteros-crear/gaveteros-crear.component';
import { GaveterosEditarComponent } from '../gaveteros-editar/gaveteros-editar.component';
import { GaveteroService } from '../../../../../services/APIS/Gavetero/gavetero.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { BuscadorComponent } from '../../../buscador/buscador.component';
import { Tabla } from '../../base/tabla';



@Component({
  selector: 'app-gaveteros-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule , GaveterosCrearComponent , GaveterosEditarComponent,AvisoEliminarComponent , MostrarerrorComponent, AvisoExitoComponent , BuscadorComponent],
  templateUrl: './gaveteros-tabla.component.html',
  styleUrls: ['./gaveteros-tabla.component.css']
})
export class GaveterosTablaComponent extends Tabla {

  botoncrear : WritableSignal<boolean> = signal(false);
  botoneditar : WritableSignal<boolean> = signal(false);

  alertaeliminar : boolean = false;
  gaveteros : Gaveteros[] = [];
  gaveteroscopia: Gaveteros[] = [];

  gaveteroSeleccionado:  Gaveteros= new Gaveteros(); 

  override columnas: string[] = ['Nombre','Tipo','Nombre Mueble','Longitud','Altura','Profundidad'];


  constructor(private gaveterosapi : GaveteroService){
    super();
  }; 




  ngOnInit(){
    this.cargarGaveteros();
  }


  limpiarGaveteroSeleccionado() {
    this.gaveteroSeleccionado = new Gaveteros();
  }



  creargarvetero() {
    this.botoncrear.set(true);
  }

  cargarGaveteros() {
 
    this.gaveterosapi.obtenerGaveteros().subscribe({
      next: (data: Gaveteros[]) => {
        this.gaveteros = data;
        this.gaveteroscopia = [...this.gaveteros]; 
      },
      error: (error) => {
        this.mensajeerror = "Error al cargar los gaveteros, intente mas tarde";
        console.error('Error al cargar los gaveteros:', error);
        this.error.set(true);
      }
    });

  }


  aplicarFiltros(event?: [string, string]){
    if (event && event[0].trim() !== '') {
      const busquedaNormalizada = this.normalizeText(event[0]);
      this.gaveteros = this.gaveteroscopia.filter(gavetero => {
        switch (event[1]) {
          case 'Nombre':
            return this.normalizeText(gavetero.Nombre || '').includes(busquedaNormalizada);
          case 'Tipo':
            return this.normalizeText(gavetero.Tipo || '').includes(busquedaNormalizada);
          case 'Nombre Mueble':
            return this.normalizeText(gavetero.NombreMueble || '').includes(busquedaNormalizada);
          case 'Longitud':
            return this.normalizeText(String(gavetero.Longitud || '')).includes(busquedaNormalizada);
          case 'Altura':
            return this.normalizeText(String(gavetero.Altura || '')).includes(busquedaNormalizada);
          case 'Profundidad':
            return this.normalizeText(String(gavetero.Profundidad || '')).includes(busquedaNormalizada);
          default:  // 'Todas las columnas'
            return this.normalizeText(gavetero.Nombre || '').includes(busquedaNormalizada) ||
                  this.normalizeText(gavetero.Tipo || '').includes(busquedaNormalizada) ||
                  this.normalizeText(gavetero.NombreMueble || '').includes(busquedaNormalizada) ||
                  this.normalizeText(String(gavetero.Longitud || '')).includes(busquedaNormalizada) ||
                  this.normalizeText(String(gavetero.Altura || '')).includes(busquedaNormalizada) ||
                  this.normalizeText(String(gavetero.Profundidad || '')).includes(busquedaNormalizada);
        }
      });
    } else {
      // Crear una copia para evitar referencias
      this.gaveteros = [...this.gaveteroscopia];
    }
  }

limpiarBusqueda(){

  this.gaveteros = [...this.gaveteroscopia]; 
  
}

editarGavetero(gavetero : Gaveteros) {
  this.botoncrear.set(false);
  this.gaveteroSeleccionado = { ...gavetero }; // Crear una copia del objeto
  this.botoneditar.set(true);
}

eliminarGavetero(gavetero : Gaveteros) {
  this.gaveteroSeleccionado = gavetero;
  this.alertaeliminar = true;
 
}

confirmarEliminacion() {
  this.gaveterosapi.eliminarGavetero(this.gaveteroSeleccionado.Id).subscribe({
    next: (response) => {
      this.mensajeexito = "Gavetero eliminado con exito";
      this.exito.set(true);
       this.cargarGaveteros(); 

    },
    error: (error) => {
      this.mensajeerror = "Error al eliminar el gavetero, intente mas tarde";
      console.error('Error al eliminar el gavetero: ' + error);
      this.error.set(true);
    }
  });
  this.limpiarGaveteroSeleccionado();
  this.alertaeliminar = false;
}

cancelarEliminacion(){
  this.alertaeliminar = false; 
  this.limpiarGaveteroSeleccionado(); 
}



}
