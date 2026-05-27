import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Equipos } from '../../../../../models/admin/Equipos';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { GrupoequipoService } from '../../../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { GrupoEquipo } from '../../../../../models/grupo_equipo';
import { Gaveteros } from '../../../../../models/admin/Gaveteros';
import { GaveteroService } from '../../../../../services/APIS/Gavetero/gavetero.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { extractErrorMessage } from '../../../../../utils/error-handler';
@Component({
  selector: 'app-equipos-crear',
  standalone: true,
  imports: [FormsModule , MostrarerrorComponent , Aviso ,AvisoExitoComponent],
  templateUrl: './equipos-crear.component.html',
  styleUrl: './equipos-crear.component.css'
})
export class EquiposCrearComponent extends BaseTablaComponent {
  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();
  grupoequipo : GrupoEquipo[] = [];
  equipo : Equipos = new Equipos();
  grupoequipoSeleccionado: GrupoEquipo | null = null;
  Gaveteros: any[] = [];
  gaveteraSeleccionada: any = null;
  constructor(private equipoapi : EquipoService , private grupoequipoAPI : GrupoequipoService , private gaveterosAPI : GaveteroService){
    super();
  }; 
  ngOnInit() {
    this.equipo.EstadoEquipo = 'operativo';
    this.cargarGruposEquipos();
    this.cargarGaveteros();
  }
  cargarGaveteros() {
    this.gaveterosAPI.obtenerGaveteros().subscribe({
      next: (data) => {
        this.Gaveteros = data;
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al cargar los gaveteros. Intente mas tarde");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
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
        const errorMsg = extractErrorMessage(error, "Error al cargar los grupos equipos. Intente mas tarde");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    });
  }
  validarcreacion(){
    if (!this.grupoequipoSeleccionado) {
      this.mensajeerror = "Debe seleccionar un grupo de equipo.";
      this.error.set(true);
      return;
    }
    this.mensajeaviso= "Esta seguro de crear el equipo?";
    this.aviso.set(true);
  }
  registrar(){
    this.equipo.IdGrupoEquipo = this.grupoequipoSeleccionado!.id;
    this.equipo.NombreGrupoEquipo = this.grupoequipoSeleccionado!.nombre;
    this.equipo.IdGavetero = this.gaveteraSeleccionada?.Id ?? null;
    this.equipo.Marca = this.grupoequipoSeleccionado!.marca ?? null;
    this.equipo.Modelo = this.grupoequipoSeleccionado!.modelo ?? null;
    this.equipoapi.crearEquipo(this.equipo).subscribe({
      next: () => {
        this.Actualizar.emit();
        this.grupoequipoSeleccionado = null;
        this.mensajeexito= "Equipo creado con exito";
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al crear el equipo. Intente mas tarde");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
       });
  }
  cerrar(){
    this.botoncrear.set(false);
  }
}
