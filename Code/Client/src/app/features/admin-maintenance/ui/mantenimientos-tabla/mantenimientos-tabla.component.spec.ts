import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { MantenimientosTablaComponent } from './mantenimientos-tabla.component';
describe('MantenimientosTablaComponent', () => {
  let component: MantenimientosTablaComponent;
  let fixture: ComponentFixture<MantenimientosTablaComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [MantenimientosTablaComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(MantenimientosTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
