import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarrerasEditarComponent } from './carreras-editar.component';

describe('CarrerasEditarComponent', () => {
  let component: CarrerasEditarComponent;
  let fixture: ComponentFixture<CarrerasEditarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarrerasEditarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarrerasEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
