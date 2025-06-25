import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';
import { AccesoriosCrearComponent } from '../accesorios-crear/accesorios-crear.component';
import { AccesoriosEditarComponent } from '../accesorios-editar/accesorios-editar.component';
import { AccesoriosService } from '../../../../../services/APIS/Accesorio/accesorios.service';



@Component({
  selector: 'app-accesorios-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule , AccesoriosCrearComponent , AccesoriosEditarComponent],
  templateUrl: './accesorios-tabla.component.html',
  styleUrls: ['./accesorios-tabla.component.css']
})
export class AccesoriosTablaComponent {

  botoncrear : WritableSignal<boolean> = signal(false);
  botoneditar : WritableSignal<boolean> = signal(false);

  alertaeliminar : boolean = false;
  accesorios : Accesorio[] = [];
  accesorioscopia: Accesorio[] = [];

  accesorioSeleccionado:  Accesorio= {
    id: 0,
    nombre: '',
    modelo: '',
    tipo: '',
    descripcion: '',
    codigo_imt: '',
    precio: 0,
    nombreEquipoAsociado: '',
    url_data_sheet: ''
  }  ;

  terminoBusqueda: string = '';


  constructor(private accesoriosapi : AccesoriosService){}; 





  ngOnInit(){
    this.cargarAccesorios();
  }


  limpiarAccesorioSeleccionado() {
    this.accesorioSeleccionado = {
      id: 0,
      nombre: '',
      modelo: '',
      tipo: '',
      descripcion: '',
      codigo_imt: '',
      precio: 0,
      nombreEquipoAsociado: '',
      url_data_sheet: ''
    };
  }



  crearaccesorio() {
    this.botoncrear.set(true);
  }

  cargarAccesorios() {
 
    this.accesoriosapi.obtenerAccesorios().subscribe(
      (data: Accesorio[]) => {
        this.accesorios = data;
        this.accesorioscopia = [...this.accesorios]; 
      },
      (error) => {
        console.error('Error al cargar los accesorios:', error);
      }
    );

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
  this.accesoriosapi.eliminarAccesorio(this.accesorioSeleccionado.id).subscribe(
    (response) => {

       this.cargarAccesorios(); 

    },
    (error) => {
      alert('Error al eliminar el accesorio: ' + error);
    }
  );
  this.limpiarAccesorioSeleccionado();
  this.alertaeliminar = false;
}

cancelarEliminacion(){
  this.alertaeliminar = false; 
  this.limpiarAccesorioSeleccionado(); 
}








}