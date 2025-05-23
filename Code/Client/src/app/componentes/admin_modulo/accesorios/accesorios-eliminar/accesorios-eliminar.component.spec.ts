import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccesoriosEliminarComponent } from './accesorios-eliminar.component';

describe('AccesoriosEliminarComponent', () => {
  let component: AccesoriosEliminarComponent;
  let fixture: ComponentFixture<AccesoriosEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccesoriosEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccesoriosEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
