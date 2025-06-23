import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificacionesComponent } from './notificaciones.component';

describe('NotificacionesComponent', () => {
  let component: NotificacionesComponent;
  let fixture: ComponentFixture<NotificacionesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotificacionesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotificacionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
