import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Carrera } from '../../../../../models/admin/Carreras';
import { CarreraService } from '../../../../../services/APIS/Carrera/carrera.service';

@Component({
  selector: 'app-carreras-editar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './carreras-editar.component.html',
  styleUrl: './carreras-editar.component.css'
})
export class CarrerasEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() carrera: Carrera = {
    Id: 0,
    Nombre: ''
  };

  constructor(private carreraService: CarreraService) {}

  confirmar() {
    if (!this.carrera.Nombre || this.carrera.Nombre.trim() === '') {
      alert('Por favor ingrese el nombre de la carrera');
      return;
    }

    this.carreraService.actualizarCarrera(this.carrera).subscribe(
      response => {
        this.actualizar.emit();
        this.cerrar();
      },
      error => {
        alert('Error al editar carrera: ' + error.message);
        this.cerrar();
      }
    );
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
