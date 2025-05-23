import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarritoPrevioComponent } from './carrito-previo.component';

describe('CarritoPrevioComponent', () => {
  let component: CarritoPrevioComponent;
  let fixture: ComponentFixture<CarritoPrevioComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarritoPrevioComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarritoPrevioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
