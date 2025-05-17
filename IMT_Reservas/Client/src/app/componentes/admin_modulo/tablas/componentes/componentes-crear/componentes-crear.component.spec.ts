import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComponentesCrearComponent } from './componentes-crear.component';

describe('ComponentesCrearComponent', () => {
  let component: ComponentesCrearComponent;
  let fixture: ComponentFixture<ComponentesCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComponentesCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ComponentesCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
