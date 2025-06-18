import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquiposTablaComponent } from './equipos-tabla.component';

describe('EquiposTablaComponent', () => {
  let component: EquiposTablaComponent;
  let fixture: ComponentFixture<EquiposTablaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EquiposTablaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EquiposTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
