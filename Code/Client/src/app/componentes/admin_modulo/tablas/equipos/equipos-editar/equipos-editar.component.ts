import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Equipos } from '../../../../../models/admin/Equipos';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { GrupoEquipo } from '../../../../../models/grupo_equipo';
import { GrupoequipoService } from '../../../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { GaveteroService } from '../../../../../services/APIS/Gavetero/gavetero.service';

@Component({
  selector: 'app-equipos-editar',
  imports: [FormsModule],
  templateUrl: './equipos-editar.component.html',
  styleUrl: './equipos-editar.component.css'
})
export class EquiposEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() equipo : Equipos = new Equipos();


  grupoequipo : GrupoEquipo[] = [];
  grupoequipoSeleccionado: GrupoEquipo | null = null;
  Gaveteros: string[] =[];

  constructor(private equipoapi: EquipoService ,  private grupoequipoAPI : GrupoequipoService , private gaveterosAPI : GaveteroService) {}; 

   
  ngOnInit() {
    this.cargarGruposEquipos();
    this.cargarGaveteros();
  }

  cargarGaveteros() {
    this.gaveterosAPI.obtenerGaveteros().subscribe({
      next: (data) => {
        this.Gaveteros = data.map(gavetero => gavetero.Nombre!);

      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    });
  }



  cargarGruposEquipos() {
    this.grupoequipoAPI.obtenersinfiltroGruposEquipos().subscribe({
      next: (data) => {
        this.grupoequipo = data;
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    });
  }


  confirmar (){
      if (this.grupoequipoSeleccionado) {
        this.equipo.NombreGrupoEquipo = this.grupoequipoSeleccionado.nombre;
        this.equipo.Marca = this.grupoequipoSeleccionado.marca ?? null;
        this.equipo.Modelo = this.grupoequipoSeleccionado.modelo ?? null;
      }

 
    this.equipoapi.editarEquipo(this.equipo).subscribe({
      next: () => {
        this.actualizar.emit();
        this.cerrar();
      },
      error: (error) => {
        alert( error.error.error + ': ' + error.error.message);
      }
        });
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
