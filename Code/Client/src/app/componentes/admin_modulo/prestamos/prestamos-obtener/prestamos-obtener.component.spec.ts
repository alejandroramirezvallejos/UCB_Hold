import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrestamosObtenerComponent } from './prestamos-obtener.component';

describe('PrestamosObtenerComponent', () => {
  let component: PrestamosObtenerComponent;
  let fixture: ComponentFixture<PrestamosObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrestamosObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrestamosObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
