import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsuariosActualizarComponent } from './usuarios-actualizar.component';

describe('UsuariosActualizarComponent', () => {
  let component: UsuariosActualizarComponent;
  let fixture: ComponentFixture<UsuariosActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsuariosActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UsuariosActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
