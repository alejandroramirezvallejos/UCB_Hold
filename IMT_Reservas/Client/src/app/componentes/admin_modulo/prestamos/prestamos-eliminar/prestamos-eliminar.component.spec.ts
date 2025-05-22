import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrestamosEliminarComponent } from './prestamos-eliminar.component';

describe('PrestamosEliminarComponent', () => {
  let component: PrestamosEliminarComponent;
  let fixture: ComponentFixture<PrestamosEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrestamosEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrestamosEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
