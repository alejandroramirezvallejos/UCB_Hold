import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsuariosEliminarComponent } from './usuarios-eliminar.component';

describe('UsuariosEliminarComponent', () => {
  let component: UsuariosEliminarComponent;
  let fixture: ComponentFixture<UsuariosEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsuariosEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UsuariosEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
