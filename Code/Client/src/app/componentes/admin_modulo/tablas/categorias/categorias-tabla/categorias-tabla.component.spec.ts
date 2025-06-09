import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoriasTablaComponent } from './categorias-tabla.component';

describe('CategoriasTablaComponent', () => {
  let component: CategoriasTablaComponent;
  let fixture: ComponentFixture<CategoriasTablaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoriasTablaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategoriasTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
