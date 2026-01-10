import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { SidebarService } from '../../services/sidebar.service';

@Component({
  selector: 'app-sidebard',
  imports: [CommonModule],
  templateUrl: './sidebard.component.html',
  styleUrl: './sidebard.component.css',
  encapsulation: ViewEncapsulation.None
})
export class SidebardComponent {

  @Input() contenido : string[] = [];

  @Output() item :EventEmitter<string> = new EventEmitter<string>();

  @Input() activeItem: string = '';

  constructor(public sidebarService: SidebarService) {}

  clickitem(item : string){
    this.item.emit(item);
  }

}
