import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Componente } from '../../../../../models/admin/Componente';
import { ComponenteService } from '../../../../../services/APIS/Componente/componente.service';

@Component({
  selector: 'app-componentes-editar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './componentes-editar.component.html',
  styleUrl: './componentes-editar.component.css'
})
export class ComponentesEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() componente: Componente = {
    Id: 0,
    Nombre: '',
    Modelo: '',
    Tipo: '',
    Descripcion: '',
    PrecioReferencia: 0,
    CodigoImtEquipo: 0,
    UrlDataSheet: '',
    NombreEquipo: ''
  };

  constructor(private componenteService: ComponenteService) {}

  confirmar() {
    this.componenteService.actualizarComponente(this.componente).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.cerrar();
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
        this.cerrar();
      }
    });
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
