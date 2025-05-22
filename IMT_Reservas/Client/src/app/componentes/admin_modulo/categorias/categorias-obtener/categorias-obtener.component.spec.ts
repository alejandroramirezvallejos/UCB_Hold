import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoriasObtenerComponent } from './categorias-obtener.component';

describe('CategoriasObtenerComponent', () => {
  let component: CategoriasObtenerComponent;
  let fixture: ComponentFixture<CategoriasObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoriasObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategoriasObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
