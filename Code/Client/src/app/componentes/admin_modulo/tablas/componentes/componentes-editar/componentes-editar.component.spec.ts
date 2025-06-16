import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComponentesEditarComponent } from './componentes-editar.component';

describe('ComponentesEditarComponent', () => {
  let component: ComponentesEditarComponent;
  let fixture: ComponentFixture<ComponentesEditarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComponentesEditarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ComponentesEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
