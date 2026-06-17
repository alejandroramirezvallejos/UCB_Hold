import { Component, HostListener, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';
import { AccesoriosService } from '../../../../../services/APIS/Accesorio/accesorios.service';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { Equipos } from '../../../../../models/admin/Equipos';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { BaseTablaComponent } from '../../base/base';
import { extractErrorMessage } from '../../../../../utils/error-handler';
import { CustomSelectComponent, OpcionSelect } from '../../../../compartidos/custom-select/custom-select.component';
@Component({
  selector: 'app-accesorios-editar',
  imports: [FormsModule , MostrarerrorComponent , AvisoExitoComponent , Aviso, CustomSelectComponent],
  templateUrl: './accesorios-editar.component.html',
  styleUrl: './accesorios-editar.component.css'
})
export class AccesoriosEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() accesorio : Accesorio = new Accesorio();
   equipos : Equipos[] = [] ;
  get equiposOpciones(): OpcionSelect[] {
    return this.equipos.map(e => ({ value: e.CodigoImt, label: `${e.NombreGrupoEquipo} ${e.Modelo} ${e.Marca} - ${e.CodigoImt}` }));
  }

  constructor(private readonly accesorioapi: AccesoriosService , private equipoAPI : EquipoService) {
    super();
  }; 

  ngOnInit(){
    this.cargarEquipos();
  }

  cargarEquipos(){
    this.equipoAPI.obtenerEquipos().subscribe({
      next: (data) => {
        this.equipos = data;
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al cargar los equipos.");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    })
  }

  confirmaredicion(){
    this.mensajeaviso="¿Está seguro que desea editar el accesorio?";
    this.aviso.set(true);
  }

  confirmar (){
    this.accesorioapi.editarAccesorio(this.accesorio).subscribe({
      next : (response) => {
        this.actualizar.emit();
        this.mensajeexito="Accesorio editado con éxito.";
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al editar el accesorio.");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    });
  }
  cerrar(){
    this.botoneditar.set(false);
  }
  @HostListener('click', ['$event'])
  onOverlayClick(event: MouseEvent) {
    if (event.target === event.currentTarget) this.cerrar();
  }

}