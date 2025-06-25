import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Componente } from '../../../../../models/admin/Componente';
import { ComponenteService } from '../../../../../services/APIS/Componente/componente.service';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { Equipos } from '../../../../../models/admin/Equipos';

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
  @Input() componente: Componente = new Componente();

  equipos : Equipos[] = [];

  constructor(private componenteService: ComponenteService, private equiposAPI : EquipoService) {}

   ngOnInit() {
    this.cargarEquipos();
  }

  cargarEquipos() {
    this.equiposAPI.obtenerEquipos().subscribe({
      next: (data: any[]) => {
        this.equipos = data;
      },
      error: (error) => {
        console.error('Error al cargar los equipos:', error.error.mensaje);
      }
    })
  }



  confirmar() {
    this.componenteService.actualizarComponente(this.componente).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.cerrar();
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    });
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
