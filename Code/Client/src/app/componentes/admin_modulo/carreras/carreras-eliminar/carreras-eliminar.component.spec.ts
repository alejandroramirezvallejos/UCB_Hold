import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarrerasEliminarComponent } from './carreras-eliminar.component';

describe('CarrerasEliminarComponent', () => {
  let component: CarrerasEliminarComponent;
  let fixture: ComponentFixture<CarrerasEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarrerasEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarrerasEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
