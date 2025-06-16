import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquiposEditarComponent } from './equipos-editar.component';

describe('EquiposEditarComponent', () => {
  let component: EquiposEditarComponent;
  let fixture: ComponentFixture<EquiposEditarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EquiposEditarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EquiposEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
