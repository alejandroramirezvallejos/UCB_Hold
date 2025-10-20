import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';
import { AccesoriosService } from '../../../../../services/APIS/Accesorio/accesorios.service';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { Equipos } from '../../../../../models/admin/Equipos';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { BaseTablaComponent } from '../../base/base';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-accesorios-crear',
  standalone: true,
  imports: [FormsModule , MostrarerrorComponent, Aviso , AvisoExitoComponent],
  templateUrl: './accesorios-crear.component.html',
  styleUrl: './accesorios-crear.component.css'
})
export class AccesoriosCrearComponent extends BaseTablaComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  equipos : Equipos[] = [] ;  

  accesorio : Accesorio = new Accesorio();


  constructor(private accesorioapi : AccesoriosService , private equipoAPI : EquipoService){
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


 
  registrar(){

    this.accesorioapi.crearAccesorio(this.accesorio).subscribe({
      next: (response )=> {
          this.Actualizar.emit(); 
          this.mensajeexito="Accesorio creado exitosamente.";
          this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror= "Error al crear el accesorio.";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
   
  }



  confirmarcreacion(){
    this.mensajeaviso="¿Está seguro que desea crear este accesorio?";
    this.aviso.set(true);
  }

  cerrar(){
    this.botoncrear.set(false);
  }

}
