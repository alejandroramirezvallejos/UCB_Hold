import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoriasActualizarComponent } from './categorias-actualizar.component';

describe('CategoriasActualizarComponent', () => {
  let component: CategoriasActualizarComponent;
  let fixture: ComponentFixture<CategoriasActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoriasActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategoriasActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
