import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsuariosObtenerComponent } from './usuarios-obtener.component';

describe('UsuariosObtenerComponent', () => {
  let component: UsuariosObtenerComponent;
  let fixture: ComponentFixture<UsuariosObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsuariosObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UsuariosObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
