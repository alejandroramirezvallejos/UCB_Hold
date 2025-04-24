import { Component } from '@angular/core';
import { PantallaMainComponent } from './pantalla-main/pantalla-main.component';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [PantallaMainComponent],
})
export class AppComponent {
  eleccionobjeto = '';

  getitem(selecteditem: string) {
    this.eleccionobjeto = selecteditem;
  }


}
