import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Componente } from '../../../../../models/admin/Componente';
import { ComponenteService } from '../../../../../services/APIS/Componente/componente.service';

@Component({
  selector: 'app-componentes-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './componentes-crear.component.html',
  styleUrl: './componentes-crear.component.css'
})
export class ComponentesCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  componente: Componente = new Componente() ;

  constructor(private componenteService: ComponenteService) {}

  registrar() {
    this.componenteService.crearComponente(this.componente).subscribe(
      response => {
        this.Actualizar.emit();
        this.cerrar();
      },
      error => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    );
  }

  cerrar() {
    this.botoncrear.set(false);
  }
}
