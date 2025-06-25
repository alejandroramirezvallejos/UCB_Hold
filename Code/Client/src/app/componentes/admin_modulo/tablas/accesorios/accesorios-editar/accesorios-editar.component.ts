import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';
import { AccesoriosService } from '../../../../../services/APIS/Accesorio/accesorios.service';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { Equipos } from '../../../../../models/admin/Equipos';

@Component({
  selector: 'app-accesorios-editar',
  imports: [FormsModule],
  templateUrl: './accesorios-editar.component.html',
  styleUrl: './accesorios-editar.component.css'
})
export class AccesoriosEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() accesorio : Accesorio = new Accesorio();


   equipos : Equipos[] = [] ;  

  constructor(private accesorioapi: AccesoriosService , private equipoAPI : EquipoService) {}; 

  
  ngOnInit(){
    this.cargarEquipos();
  }

  cargarEquipos(){
    this.equipoAPI.obtenerEquipos().subscribe({
      next: (data) => {
        this.equipos = data;
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    })
  }




  confirmar (){
 
    this.accesorioapi.editarAccesorio(this.accesorio).subscribe({
      next : (response) => {
        this.actualizar.emit();
        this.cerrar();
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    });

    
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
