import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';
import { AccesoriosCrearComponent } from '../accesorios-crear/accesorios-crear.component';
import { AccesoriosEditarComponent } from '../accesorios-editar/accesorios-editar.component';
import { AccesoriosService } from '../../../../../services/APIS/Accesorio/accesorios.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';



@Component({
  selector: 'app-accesorios-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule , AccesoriosCrearComponent , AccesoriosEditarComponent , AvisoEliminarComponent , MostrarerrorComponent , AvisoExitoComponent , Aviso],
  templateUrl: './accesorios-tabla.component.html',
  styleUrls: ['./accesorios-tabla.component.css']
})
export class AccesoriosTablaComponent  extends BaseTablaComponent{

  botoncrear : WritableSignal<boolean> = signal(false);
  botoneditar : WritableSignal<boolean> = signal(false);

  alertaeliminar : boolean = false;
  accesorios : Accesorio[] = [];
  accesorioscopia: Accesorio[] = [];

  accesorioSeleccionado:  Accesorio= new Accesorio();

  terminoBusqueda: string = '';


  constructor(private accesoriosapi : AccesoriosService){
    super();
  }; 





  ngOnInit(){
    this.cargarAccesorios();
  }


  limpiarAccesorioSeleccionado() {
    this.accesorioSeleccionado = new Accesorio();

  }

  crearaccesorio() {
    this.botoncrear.set(true);
  }

  cargarAccesorios() {
 
    this.accesoriosapi.obtenerAccesorios().subscribe({
      next: (data: Accesorio[]) => {
        this.accesorios = data;
        this.accesorioscopia = [...this.accesorios]; 
      },
      error: (error) => {
        this.mensajeerror = 'Error al cargar los accesorios. Por favor, intente más tarde.';
        console.error('Error al cargar los accesorios:', error);
        this.error.set(true);
      }
    });

  }

buscar(){
  if(this.terminoBusqueda.trim() === '') {
    this.limpiarBusqueda(); 
    return ; 
  }

  this.accesorios = this.accesorioscopia.filter(accesorio =>
    (accesorio.nombre || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase() ) ||
    (accesorio.modelo|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    (accesorio.tipo|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    String(accesorio.codigo_imt || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    (accesorio.nombreEquipoAsociado || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) 
  );


}

limpiarBusqueda(){
  this.terminoBusqueda = '';
  this.accesorios = [...this.accesorioscopia]; 
  
}

editarAccesorio(accesorio : Accesorio) {
  this.botoncrear.set(false);
  this.accesorioSeleccionado = { ...accesorio }; // Crear una copia del objeto
  this.botoneditar.set(true);
}

eliminarAccesorio(accesorio : Accesorio) {
  this.accesorioSeleccionado = accesorio;
  this.alertaeliminar = true;
 
}

confirmarEliminacion() {
  this.accesoriosapi.eliminarAccesorio(this.accesorioSeleccionado.Id).subscribe({
    next: (response) => {

       this.cargarAccesorios(); 

    },
    error: (error) => {
      this.mensajeerror = 'Error al eliminar el accesorio. Por favor, intente más tarde.';
      alert('Error al eliminar el accesorio: ' + error);
      this.error.set(true);
    }
  });
  this.limpiarAccesorioSeleccionado();
  this.alertaeliminar = false;
}

cancelarEliminacion(){
  this.alertaeliminar = false; 
  this.limpiarAccesorioSeleccionado(); 
}








}