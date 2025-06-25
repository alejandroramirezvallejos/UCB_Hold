import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Equipos } from '../../../../../models/admin/Equipos';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { GrupoequipoService } from '../../../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { GrupoEquipo } from '../../../../../models/grupo_equipo';
import { Gaveteros } from '../../../../../models/admin/Gaveteros';
import { GaveteroService } from '../../../../../services/APIS/Gavetero/gavetero.service';

@Component({
  selector: 'app-equipos-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './equipos-crear.component.html',
  styleUrl: './equipos-crear.component.css'
})
export class EquiposCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  grupoequipo : GrupoEquipo[] = [];
  equipo : Equipos = new Equipos();

  grupoequipoSeleccionado: GrupoEquipo | null = null;
  
  Gaveteros: string[] =[];

  constructor(private equipoapi : EquipoService , private grupoequipoAPI : GrupoequipoService , private gaveterosAPI : GaveteroService){}; 

  
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

  registrar(){
      if (!this.grupoequipoSeleccionado) {
      alert('Debe seleccionar un grupo de equipo.');
      return;
    }
    this.equipo.NombreGrupoEquipo = this.grupoequipoSeleccionado.nombre;
    this.equipo.Marca = this.grupoequipoSeleccionado.marca ?? null  ;
    this.equipo.Modelo = this.grupoequipoSeleccionado.modelo ?? null;

    console.log('Equipo a enviar:', this.equipo);

    this.equipoapi.crearEquipo(this.equipo).subscribe({
      next: () => {
        this.Actualizar.emit();
        this.grupoequipoSeleccionado = null;
        this.cerrar();
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
       });
   
  }

  cerrar(){
    this.botoncrear.set(false);
  }

}
