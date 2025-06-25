import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Gaveteros } from '../../../../../models/admin/Gaveteros';
import { GaveteroService } from '../../../../../services/APIS/Gavetero/gavetero.service';
import { MuebleService } from '../../../../../services/APIS/Mueble/mueble.service';

@Component({
  selector: 'app-gaveteros-editar',
  imports: [FormsModule],
  templateUrl: './gaveteros-editar.component.html',
  styleUrl: './gaveteros-editar.component.css'
})
export class GaveterosEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() gavetero : Gaveteros =new Gaveteros();

  muebles : string[] = [];

  constructor(private gaveteroapi: GaveteroService, private mueblesAPI : MuebleService) {}; 

  
  ngOnInit(){
    this.cargarMuebles();
  }

  cargarMuebles(){
    this.mueblesAPI.obtenerMuebles().subscribe({
      next: (data) => {
        this.muebles = data.map(mueble => mueble.Nombre);
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    })
  }

  confirmar (){
 
    this.gaveteroapi.editarGavetero(this.gavetero).subscribe({
      next: () => {
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
