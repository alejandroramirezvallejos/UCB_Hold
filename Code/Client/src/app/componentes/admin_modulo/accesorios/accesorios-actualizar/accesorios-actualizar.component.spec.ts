import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccesoriosActualizarComponent } from './accesorios-actualizar.component';

describe('AccesoriosActualizarComponent', () => {
  let component: AccesoriosActualizarComponent;
  let fixture: ComponentFixture<AccesoriosActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccesoriosActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccesoriosActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
