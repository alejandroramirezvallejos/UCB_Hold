import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Componente } from '../../../../../models/admin/Componente';
import { ComponenteService } from '../../../../../services/APIS/Componente/componente.service';
import { Equipos } from '../../../../../models/admin/Equipos';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { extractErrorMessage } from '../../../../../utils/error-handler';
@Component({
  selector: 'app-componentes-crear',
  standalone: true,
  imports: [FormsModule , MostrarerrorComponent , AvisoExitoComponent , Aviso],
  templateUrl: './componentes-crear.component.html',
  styleUrl: './componentes-crear.component.css'
})
export class ComponentesCrearComponent extends BaseTablaComponent {
  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();
  equipos : Equipos[] = [];
  componente: Componente = new Componente() ;
  constructor(private componenteService: ComponenteService , private equiposAPI : EquipoService) {
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
        const errorMsg = extractErrorMessage(error, "Error al obtener los equipos , intente mas tarde");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    })
  }
  validarregistro(){
    this.mensajeaviso="Estas seguro de crear este componente?";
    this.aviso.set(true);
  }
  registrar() {
    this.componenteService.crearComponente(this.componente).subscribe({
      next: (response) => {
        this.Actualizar.emit();
        this.mensajeexito = "Componente creado con exito";
        this.exito.set(true);
      },
      error : (error) => {
        this.mensajeerror = "Error al crear el componente , Intente mas tarde ";
        this.error.set(true);
        console.error(error?.error?.error + ': ' + error?.error?.message);
      }
    });
  }
  cerrar() {
    this.botoncrear.set(false);
  }
}
