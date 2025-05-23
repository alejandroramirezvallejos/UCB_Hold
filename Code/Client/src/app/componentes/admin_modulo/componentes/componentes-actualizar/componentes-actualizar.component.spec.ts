import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComponentesActualizarComponent } from './componentes-actualizar.component';

describe('ComponentesActualizarComponent', () => {
  let component: ComponentesActualizarComponent;
  let fixture: ComponentFixture<ComponentesActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComponentesActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ComponentesActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
