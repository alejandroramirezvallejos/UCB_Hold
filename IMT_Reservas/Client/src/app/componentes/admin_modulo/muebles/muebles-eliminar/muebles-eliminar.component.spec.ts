import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MueblesEliminarComponent } from './muebles-eliminar.component';

describe('MueblesEliminarComponent', () => {
  let component: MueblesEliminarComponent;
  let fixture: ComponentFixture<MueblesEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MueblesEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MueblesEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
