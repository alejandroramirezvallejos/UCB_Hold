import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComponentesObtenerComponent } from './componentes-obtener.component';

describe('ComponentesObtenerComponent', () => {
  let component: ComponentesObtenerComponent;
  let fixture: ComponentFixture<ComponentesObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComponentesObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ComponentesObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
