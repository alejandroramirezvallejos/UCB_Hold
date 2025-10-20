import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Equipos } from '../../../../../models/admin/Equipos';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { GrupoEquipo } from '../../../../../models/grupo_equipo';
import { GrupoequipoService } from '../../../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { GaveteroService } from '../../../../../services/APIS/Gavetero/gavetero.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-equipos-editar',
  imports: [FormsModule , MostrarerrorComponent , Aviso , AvisoExitoComponent],
  templateUrl: './equipos-editar.component.html',
  styleUrl: './equipos-editar.component.css'
})
export class EquiposEditarComponent extends BaseTablaComponent{
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() equipo : Equipos = new Equipos();


  grupoequipo : GrupoEquipo[] = [];
  grupoequipoSeleccionado: GrupoEquipo | null = null;
  Gaveteros: string[] =[];

  constructor(private equipoapi: EquipoService ,  private grupoequipoAPI : GrupoequipoService , private gaveterosAPI : GaveteroService) {
    super(); 
  }; 

   
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
        this.mensajeerror = "Error al cargar gaveteros";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }



  cargarGruposEquipos() {
    this.grupoequipoAPI.obtenersinfiltroGruposEquipos().subscribe({
      next: (data) => {
        this.grupoequipo = data;
      },
      error: (error) => {
        this.mensajeerror = "Error al cargar grupos de equipos";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  validaredicion(){
      this.mensajeaviso="¿Confirma que desea editar el equipo "+ this.equipo.NombreGrupoEquipo + " ?";
      this.aviso.set(true);
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
        this.mensajeexito="Equipo editado con exito";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror = "Error al editar el equipo";
        console.error( error.error.error + ': ' + error.error.message);
        this.error.set(true);
      }
        });
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
