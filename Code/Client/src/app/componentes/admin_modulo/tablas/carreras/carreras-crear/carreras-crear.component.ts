import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CarreraService } from '../../../../../services/APIS/Carrera/carrera.service';

@Component({
  selector: 'app-carreras-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './carreras-crear.component.html',
  styleUrl: './carreras-crear.component.css'
})
export class CarrerasCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  nombreCarrera: string = '';

  constructor(private carreraService: CarreraService) {}

  registrar() {
    if (this.nombreCarrera.trim() === '') {
      alert('Por favor ingrese el nombre de la carrera');
      return;
    }

    this.carreraService.crearCarrera(this.nombreCarrera).subscribe(
      response => {
        this.Actualizar.emit();
        this.cerrar();
      },
      error => {
        alert('Error al crear carrera: ' + error);
        this.cerrar();
      }
    );
  }

  cerrar() {
    this.nombreCarrera = '';
    this.botoncrear.set(false);
  }
}
