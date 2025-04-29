// objeto.component.ts
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-objeto',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './objeto.component.html',
  styleUrl: './objeto.component.css'
})
export class ObjetoComponent {
  @Input() id: string = ''


  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    // Se captura el parámetro "nombre" de la URL
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.id = id;

    }







  }
}

