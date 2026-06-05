import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';
import { AccesoriosService } from '../../../../../services/APIS/Accesorio/accesorios.service';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { Equipos } from '../../../../../models/admin/Equipos';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { BaseTablaComponent } from '../../base/base';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { extractErrorMessage } from '../../../../../utils/error-handler';
import { CustomSelectComponent, OpcionSelect } from '../../../../compartidos/custom-select/custom-select.component';
@Component({
  selector: 'app-accesorios-crear',
  standalone: true,
  imports: [FormsModule , MostrarerrorComponent, Aviso , AvisoExitoComponent, CustomSelectComponent],
  templateUrl: './accesorios-crear.component.html',
  styleUrl: './accesorios-crear.component.css'
})
export class AccesoriosCrearComponent extends BaseTablaComponent {
  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();
  equipos : Equipos[] = [] ;
  accesorio : Accesorio = new Accesorio();
  get equiposOpciones(): OpcionSelect[] {
    return this.equipos.map(e => ({ value: e.CodigoImt, label: `${e.NombreGrupoEquipo} ${e.Modelo} ${e.Marca} - ${e.CodigoImt}` }));
  }

  constructor(private accesorioapi : AccesoriosService , private equipoAPI : EquipoService){
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


  registrar(){
    this.accesorioapi.crearAccesorio(this.accesorio).subscribe({
      next: (response )=> {
          this.Actualizar.emit(); 
          this.mensajeexito="Accesorio creado exitosamente.";
          this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al crear el accesorio.");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    });
  }


  confirmarcreacion(){
    this.mensajeaviso="¿Está seguro que desea crear este accesorio?";
    this.aviso.set(true);
  }
  
  cerrar(){
    this.botoncrear.set(false);
  }
}
