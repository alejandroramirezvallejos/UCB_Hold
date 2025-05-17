import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoriasCrearComponent } from './categorias-crear.component';

describe('CategoriasCrearComponent', () => {
  let component: CategoriasCrearComponent;
  let fixture: ComponentFixture<CategoriasCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoriasCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategoriasCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
