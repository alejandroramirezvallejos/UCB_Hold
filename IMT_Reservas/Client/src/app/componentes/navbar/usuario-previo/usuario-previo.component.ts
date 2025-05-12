import { Component, Input, signal, WritableSignal } from '@angular/core';

@Component({
  selector: 'app-usuario-previo',
  imports: [],
  templateUrl: './usuario-previo.component.html',
  styleUrl: './usuario-previo.component.css'
})
export class UsuarioPrevioComponent {

  @Input() showUserMenu : WritableSignal<Boolean> = signal(true); 
 
    seleccionar(item: string) {
    
    
    
    
      this.showUserMenu.set(false); 
  }
}
