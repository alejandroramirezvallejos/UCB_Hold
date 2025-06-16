import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccesoriosEditarComponent } from './accesorios-editar.component';

describe('AccesoriosEditarComponent', () => {
  let component: AccesoriosEditarComponent;
  let fixture: ComponentFixture<AccesoriosEditarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccesoriosEditarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccesoriosEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
