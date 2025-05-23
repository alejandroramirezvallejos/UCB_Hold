import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComponentesEliminarComponent } from './componentes-eliminar.component';

describe('ComponentesEliminarComponent', () => {
  let component: ComponentesEliminarComponent;
  let fixture: ComponentFixture<ComponentesEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComponentesEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ComponentesEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
