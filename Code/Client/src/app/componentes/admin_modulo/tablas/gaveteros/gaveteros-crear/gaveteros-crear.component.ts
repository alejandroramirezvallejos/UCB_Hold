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
  selector: 'app-gaveteros-crear',
  standalone: true,
  imports: [FormsModule, MostrarerrorComponent , Aviso , AvisoExitoComponent],
  templateUrl: './gaveteros-crear.component.html',
  styleUrl: './gaveteros-crear.component.css'
})
export class GaveterosCrearComponent extends BaseTablaComponent{

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  muebles : string[] = [];
  gavetero : Gaveteros = new Gaveteros();


  constructor(private gaveteroapi : GaveteroService , private mueblesAPI : MuebleService){
    super();
  }; 

  ngOnInit(){
    this.cargarMuebles();
  }

  cargarMuebles(){
    this.mueblesAPI.obtenerMuebles().subscribe({
      next: (data) => {
        this.muebles = data.map(mueble => mueble.Nombre);
      },
      error: (error) => {
        this.mensajeerror = "Error al cargar los muebles";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    })
  }

  validarregistro(){
    this.mensajeaviso = "¿Desea registrar el gavetero?";
    this.aviso.set(true);
  }


  registrar(){

    this.gaveteroapi.crearGavetero(this.gavetero).subscribe({
      next : (response) => {
        this.Actualizar.emit(); 
        this.mensajeexito = "Gavetero registrado con exito";
        this.exito.set(true);
      },
      error:  (error) => {
        this.mensajeerror = "Error al registrar el gavetero";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
  });
   
  }

  cerrar(){
    this.botoncrear.set(false);
  }

}
