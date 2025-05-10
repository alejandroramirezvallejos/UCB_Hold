import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComponentesComponent } from './componentes.component';

describe('ComponentesComponent', () => {
  let component: ComponentesComponent;
  let fixture: ComponentFixture<ComponentesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComponentesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ComponentesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
