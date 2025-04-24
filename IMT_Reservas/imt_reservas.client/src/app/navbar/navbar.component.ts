// navbar.component.ts
import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  @Output() seleccion = new EventEmitter<string>();
  showUserMenu = false;

  toggleUserMenu() {
    this.showUserMenu = !this.showUserMenu;
  }

  seleccionar(item: string) {
    this.seleccion.emit(item);
    this.showUserMenu = false; // Cierra el menú al seleccionar
  }
}
