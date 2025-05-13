import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MueblesComponent } from './muebles.component';

describe('MueblesComponent', () => {
  let component: MueblesComponent;
  let fixture: ComponentFixture<MueblesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MueblesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MueblesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
