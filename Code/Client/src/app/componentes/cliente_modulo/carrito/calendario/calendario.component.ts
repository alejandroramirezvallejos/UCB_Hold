import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-calendario',
  imports: [CommonModule],
  templateUrl: './calendario.component.html',
  styleUrls: ['./calendario.component.css']
})
export class CalendarioComponent {

 diasDelMes: Date[] = [];

 mesActual: Date = new Date();

  ngOnInit(): void { 
    this.generarDiasDelMes();
  }

 generarDiasDelMes(): void {
    const primerDia = new Date(this.mesActual.getFullYear(), this.mesActual.getMonth(), 1);
    const ultimoDia = new Date(this.mesActual.getFullYear(), this.mesActual.getMonth() + 1, 0);
    
    this.diasDelMes = [];
    for (let d = new Date(primerDia); d <= ultimoDia; d.setDate(d.getDate() + 1)) {
      this.diasDelMes.push(new Date(d));
    }
  }

  cambiarMes(valor : number){
    this.mesActual = new Date(this.mesActual.getFullYear(), this.mesActual.getMonth() + valor, 1);
    this.generarDiasDelMes();
  }

}
