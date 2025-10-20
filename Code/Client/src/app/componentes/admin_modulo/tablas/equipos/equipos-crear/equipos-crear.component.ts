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
  
  Gaveteros: string[] =[];

  constructor(private equipoapi : EquipoService , private grupoequipoAPI : GrupoequipoService , private gaveterosAPI : GaveteroService){
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
        this.mensajeerror= "Error al cargar los gaveteros. Intente mas tarde";
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
         this.mensajeerror= "Error al cargar los grupos equipos. Intente mas tarde";
        console.error(error.error.error + ': ' + error.error.mensaje);
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
    
    this.equipo.NombreGrupoEquipo = this.grupoequipoSeleccionado!.nombre;
    this.equipo.Marca = this.grupoequipoSeleccionado!.marca ?? null  ;
    this.equipo.Modelo = this.grupoequipoSeleccionado!.modelo ?? null;

    console.log('Equipo a enviar:', this.equipo);

    this.equipoapi.crearEquipo(this.equipo).subscribe({
      next: () => {
        this.Actualizar.emit();
        this.grupoequipoSeleccionado = null;
        this.mensajeexito= "Equipo creado con exito";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror= "Error al crear el equipo. Intente mas tarde";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
       });
   
  }

  cerrar(){
    this.botoncrear.set(false);
  }

}
