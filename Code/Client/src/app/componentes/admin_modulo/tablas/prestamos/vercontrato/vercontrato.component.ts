import { Component, Input, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PrestamosAPIService } from '../../../../../services/APIS/prestamo/prestamos-api.service';

@Component({
  selector: 'app-vercontrato',
  imports: [CommonModule],
  templateUrl: './vercontrato.component.html',
  styleUrl: './vercontrato.component.css'
})
export class VercontratoComponent {
  @Input() vercontraro : WritableSignal<boolean> = signal(true);
  @Input() idprestamo : number = 0;
  contratoContent: string = '';


   constructor(private prestamo : PrestamosAPIService){};

  ngOnInit() {
    this.cargarcontrato(); 
  }

  cargarcontrato(){
    this.prestamo.obtenercontratoPrestamo(this.idprestamo).subscribe({
      next: (data) => {
        this.contratoContent = data;
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    });


  }

  cerrar(){
    this.vercontraro.set(false);
  }
}
