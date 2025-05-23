import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MueblesActualizarComponent } from './muebles-actualizar.component';

describe('MueblesActualizarComponent', () => {
  let component: MueblesActualizarComponent;
  let fixture: ComponentFixture<MueblesActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MueblesActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MueblesActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
