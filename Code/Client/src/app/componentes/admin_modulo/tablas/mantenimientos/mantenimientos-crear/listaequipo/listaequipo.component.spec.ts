import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaequipoComponent } from './listaequipo.component';

describe('ListaequipoComponent', () => {
  let component: ListaequipoComponent;
  let fixture: ComponentFixture<ListaequipoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaequipoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListaequipoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
