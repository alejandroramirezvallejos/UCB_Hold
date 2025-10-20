import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Gaveteros } from '../../../../../models/admin/Gaveteros';
import { GaveteroService } from '../../../../../services/APIS/Gavetero/gavetero.service';
import { MuebleService } from '../../../../../services/APIS/Mueble/mueble.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-gaveteros-editar',
  imports: [FormsModule, MostrarerrorComponent , Aviso ,AvisoExitoComponent],
  templateUrl: './gaveteros-editar.component.html',
  styleUrl: './gaveteros-editar.component.css'
})
export class GaveterosEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() gavetero : Gaveteros =new Gaveteros();

  muebles : string[] = [];

  constructor(private gaveteroapi: GaveteroService, private mueblesAPI : MuebleService) {super(); }; 

  
  ngOnInit(){
    this.cargarMuebles();
  }

  cargarMuebles(){
    this.mueblesAPI.obtenerMuebles().subscribe({
      next: (data) => {
        this.muebles = data.map(mueble => mueble.Nombre);
      },
      error: (error) => {
        this.mensajeerror = "Error al cargar los muebles, intente mas tarde";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    })
  }


  validaredicion(){
    this.mensajeaviso="¿Desea confirmar la edición del gavetero?";
    this.aviso.set(true);
  }

  confirmar (){
 
    this.gaveteroapi.editarGavetero(this.gavetero).subscribe({
      next: () => {
        this.actualizar.emit();
        this.mensajeexito="Gavetero editado con éxito";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror = "Error al editar el gavetero, intente mas tarde";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
