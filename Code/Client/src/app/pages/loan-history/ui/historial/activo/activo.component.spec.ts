import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { ActivoComponent } from './activo.component';
describe('ActivoComponent', () => {
  let component: ActivoComponent;
  let fixture: ComponentFixture<ActivoComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [ActivoComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(ActivoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
