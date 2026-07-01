import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { CarrerasTablaComponent } from './carreras-tabla.component';
describe('CarrerasTablaComponent', () => {
  let component: CarrerasTablaComponent;
  let fixture: ComponentFixture<CarrerasTablaComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [CarrerasTablaComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(CarrerasTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
