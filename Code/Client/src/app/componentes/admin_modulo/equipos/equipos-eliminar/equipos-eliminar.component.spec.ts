import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquiposEliminarComponent } from './equipos-eliminar.component';

describe('EquiposEliminarComponent', () => {
  let component: EquiposEliminarComponent;
  let fixture: ComponentFixture<EquiposEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EquiposEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EquiposEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
