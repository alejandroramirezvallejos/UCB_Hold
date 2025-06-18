import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsuariosTablaComponent } from './usuarios-tabla.component';

describe('UsuariosTablaComponent', () => {
  let component: UsuariosTablaComponent;
  let fixture: ComponentFixture<UsuariosTablaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsuariosTablaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UsuariosTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
