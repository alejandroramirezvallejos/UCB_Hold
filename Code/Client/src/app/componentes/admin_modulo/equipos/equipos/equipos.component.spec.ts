import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquiposComponent } from './equipos.component';

describe('EquiposComponent', () => {
  let component: EquiposComponent;
  let fixture: ComponentFixture<EquiposComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EquiposComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EquiposComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
