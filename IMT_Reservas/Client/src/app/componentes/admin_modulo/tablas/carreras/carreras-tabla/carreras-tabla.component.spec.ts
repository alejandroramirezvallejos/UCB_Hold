import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarrerasTablaComponent } from './carreras-tabla.component';

describe('CarrerasTablaComponent', () => {
  let component: CarrerasTablaComponent;
  let fixture: ComponentFixture<CarrerasTablaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarrerasTablaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarrerasTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
