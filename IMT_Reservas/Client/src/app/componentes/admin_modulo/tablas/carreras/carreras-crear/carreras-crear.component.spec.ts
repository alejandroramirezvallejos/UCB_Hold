import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarrerasCrearComponent } from './carreras-crear.component';

describe('CarrerasCrearComponent', () => {
  let component: CarrerasCrearComponent;
  let fixture: ComponentFixture<CarrerasCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarrerasCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarrerasCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
