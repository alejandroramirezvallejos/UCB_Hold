import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';
import { AccesoriosService } from '../../../../../services/APIS/Accesorio/accesorios.service';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { Equipos } from '../../../../../models/admin/Equipos';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { BaseTablaComponent } from '../../base/base';

@Component({
  selector: 'app-accesorios-editar',
  imports: [FormsModule , MostrarerrorComponent , AvisoExitoComponent , Aviso],
  templateUrl: './accesorios-editar.component.html',
  styleUrl: './accesorios-editar.component.css'
})
export class AccesoriosEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() accesorio : Accesorio = new Accesorio();


   equipos : Equipos[] = [] ;  

  constructor(private accesorioapi: AccesoriosService , private equipoAPI : EquipoService) {
    super();
  }; 

  
  ngOnInit(){
    this.cargarEquipos();
  }

  cargarEquipos(){
    this.equipoAPI.obtenerEquipos().subscribe({
      next: (data) => {
        this.equipos = data;
      },
      error: (error) => {
        this.mensajeerror= "Error al cargar los equipos.";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    })
  }

  confirmaredicion(){
    this.mensajeaviso="¿Está seguro que desea editar el accesorio?";
    this.aviso.set(true);
  }


  confirmar (){
 
    this.accesorioapi.editarAccesorio(this.accesorio).subscribe({
      next : (response) => {
        this.actualizar.emit();
        this.mensajeexito="Accesorio editado con éxito.";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror= "Error al editar el accesorio.";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });

    
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
