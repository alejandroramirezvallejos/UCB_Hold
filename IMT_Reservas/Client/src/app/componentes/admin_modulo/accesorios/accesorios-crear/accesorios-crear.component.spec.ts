import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccesoriosCrearComponent } from './accesorios-crear.component';

describe('AccesoriosCrearComponent', () => {
  let component: AccesoriosCrearComponent;
  let fixture: ComponentFixture<AccesoriosCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccesoriosCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccesoriosCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
