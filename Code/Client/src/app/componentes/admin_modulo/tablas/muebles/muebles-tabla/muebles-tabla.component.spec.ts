import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MueblesTablaComponent } from './muebles-tabla.component';

describe('MueblesTablaComponent', () => {
  let component: MueblesTablaComponent;
  let fixture: ComponentFixture<MueblesTablaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MueblesTablaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MueblesTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
