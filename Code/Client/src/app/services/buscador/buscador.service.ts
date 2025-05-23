import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BuscadorService {
  producto: string = '';
  categoria: string = '';
  constructor() { }

  reiniciar() {
    this.producto = '';
    this.categoria = '';
  }

}
