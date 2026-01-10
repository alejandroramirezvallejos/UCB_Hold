import { Injectable, signal, WritableSignal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SidebarService {
  isOpen: WritableSignal<boolean> = signal(false);

  toggle() {
    this.isOpen.update(val => !val);
  }

  close() {
    this.isOpen.set(false);
  }
}

