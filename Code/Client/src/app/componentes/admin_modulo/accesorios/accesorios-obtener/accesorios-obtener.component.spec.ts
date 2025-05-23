import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccesoriosObtenerComponent } from './accesorios-obtener.component';

describe('AccesoriosObtenerComponent', () => {
  let component: AccesoriosObtenerComponent;
  let fixture: ComponentFixture<AccesoriosObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccesoriosObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccesoriosObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
