import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrestamosEditarComponent } from './prestamos-editar.component';

describe('PrestamosEditarComponent', () => {
  let component: PrestamosEditarComponent;
  let fixture: ComponentFixture<PrestamosEditarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrestamosEditarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrestamosEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
