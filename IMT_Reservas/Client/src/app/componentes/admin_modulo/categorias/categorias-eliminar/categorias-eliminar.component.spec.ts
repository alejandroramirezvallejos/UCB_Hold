import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoriasEliminarComponent } from './categorias-eliminar.component';

describe('CategoriasEliminarComponent', () => {
  let component: CategoriasEliminarComponent;
  let fixture: ComponentFixture<CategoriasEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoriasEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategoriasEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
