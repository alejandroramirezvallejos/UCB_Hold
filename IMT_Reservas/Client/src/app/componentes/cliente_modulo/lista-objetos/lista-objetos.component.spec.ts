import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaObjetosComponent } from './lista-objetos.component';

describe('ListaObjetosComponent', () => {
  let component: ListaObjetosComponent;
  let fixture: ComponentFixture<ListaObjetosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaObjetosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListaObjetosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
