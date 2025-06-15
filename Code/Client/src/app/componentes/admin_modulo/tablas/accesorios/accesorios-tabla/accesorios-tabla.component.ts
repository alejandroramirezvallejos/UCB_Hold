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
  valoreliminar: number =0;
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
    nombreEquipoAsociado: ''
  }  ;

  terminoBusqueda: string = '';


  constructor(private accesoriosapi : AccesoriosService){}; 


  // ----
  sortColumn: string = 'nombre';

  sortDirection: 'asc' | 'desc' = 'asc';


  ngOnInit(){
    this.cargarAccesorios();
  }


  crearaccesorio() {
    this.botoncrear.set(true);
  }

  cargarAccesorios() {
    // Simulación de carga de accesorios, en un caso real se haría una llamada a un servicio
    this.accesoriosapi.obtenerAccesorios().subscribe(
      (data: Accesorio[]) => {
        this.accesorios = data;
        this.accesorioscopia = [...this.accesorios]; // Guardar una copia para la búsqueda
      },
      (error) => {
        console.error('Error al cargar los accesorios:', error);
      }
    );

  }

buscar(){
  if(this.terminoBusqueda.trim() === '') {
    this.limpiarBusqueda(); 
  }

  this.accesorios = this.accesorios.filter(accesorio =>
    accesorio.nombre.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    accesorio.modelo.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    accesorio.tipo.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    accesorio.codigo_imt.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
    accesorio.nombreEquipoAsociado?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) 
  );
}

limpiarBusqueda(){
  this.terminoBusqueda = '';
  this.accesorios = [...this.accesorioscopia]; 
  
}

editarAccesorio(accesorio : Accesorio) {
  this.botoncrear.set(false);
  this.accesorioSeleccionado = accesorio;
  this.botoneditar.set(true);
}

eliminarAccesorio(i : number) {
  this.valoreliminar = i;
  this.alertaeliminar = true;
 
}

confirmarEliminacion() {
  this.accesorios.splice(this.valoreliminar, 1);
  this.alertaeliminar = false;
  this.valoreliminar = 0; 
}

cancelarEliminacion(){
  this.alertaeliminar = false; 
  this.valoreliminar = 0; 
}





// nose que hace
aplicarOrdenamiento() {
  this.accesorios.sort((a, b) => {
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