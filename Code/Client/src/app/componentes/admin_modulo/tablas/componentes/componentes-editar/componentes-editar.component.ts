import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Componente } from '../../../../../models/admin/Componente';
import { ComponenteService } from '../../../../../services/APIS/Componente/componente.service';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { Equipos } from '../../../../../models/admin/Equipos';
import { BaseTablaComponent } from '../../base/base';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';

@Component({
  selector: 'app-componentes-editar',
  standalone: true,
  imports: [FormsModule, MostrarerrorComponent, AvisoExitoComponent, Aviso],
  templateUrl: './componentes-editar.component.html',
  styleUrl: './componentes-editar.component.css'
})
export class ComponentesEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() componente: Componente = new Componente();

  equipos : Equipos[] = [];

  constructor(private componenteService: ComponenteService, private equiposAPI : EquipoService) {
    super();
  }

   ngOnInit() {
    this.cargarEquipos();
  }

  cargarEquipos() {
    this.equiposAPI.obtenerEquipos().subscribe({
      next: (data: any[]) => {
        this.equipos = data;
      },
      error: (error) => {
        this.mensajeerror="Error al obtener los equipos , intente mas tarde";
        console.error('Error al cargar los equipos:', error.error.mensaje);
        this.error.set(true);
      }
    })
  }

    validaredicion(){
      this.mensajeaviso="Estas seguro de editar este componente?";
      this.aviso.set(true);
    }


  confirmar() {
    this.componenteService.actualizarComponente(this.componente).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.mensajeexito="Componente actualizado satisfactoriamente";
        this.exito.set(true); 
      },
      error: (error) => {
        this.mensajeerror="Error al actualizar el componenete , intente mas tarde"
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true); 
      }
    });
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
