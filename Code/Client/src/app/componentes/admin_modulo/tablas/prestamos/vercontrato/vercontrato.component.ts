import { Component, Input, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PrestamosAPIService } from '../../../../../services/APIS/prestamo/prestamos-api.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
@Component({
  selector: 'app-vercontrato',
  imports: [CommonModule, MostrarerrorComponent],
  templateUrl: './vercontrato.component.html',
  styleUrl: './vercontrato.component.css'
})
export class VercontratoComponent extends BaseTablaComponent {
  @Input() vercontraro : WritableSignal<boolean> = signal(true);
  @Input() idprestamo : number = 0;
  contratoContent: string = '';
   constructor(private prestamo : PrestamosAPIService){
    super();
   };
  ngOnInit() {
    this.cargarcontrato();
  }
  cargarcontrato(){
    this.prestamo.obtenercontratoPrestamo(this.idprestamo).subscribe({
      next: (data) => {
        if (!data || typeof data !== 'string' || data.trim() === '') {
          this.mensajeerror = 'El contrato no está disponible.';
          this.error.set(true);
          return;
        }
        this.contratoContent = data;
      },
      error: (error) => {
        this.mensajeerror="No se pudo cargar el contrato del prestamo.";
        console.error(error);
        this.error.set(true);
      }
    });
  }
  cerrar(){
    this.vercontraro.set(false);
  }
}
