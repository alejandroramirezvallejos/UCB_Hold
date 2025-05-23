import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrestamosCrearComponent } from './prestamos-crear.component';

describe('PrestamosCrearComponent', () => {
  let component: PrestamosCrearComponent;
  let fixture: ComponentFixture<PrestamosCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrestamosCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrestamosCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
