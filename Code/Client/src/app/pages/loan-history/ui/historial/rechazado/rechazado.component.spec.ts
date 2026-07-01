import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { RechazadoComponent } from './rechazado.component';
describe('RechazadoComponent', () => {
  let component: RechazadoComponent;
  let fixture: ComponentFixture<RechazadoComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [RechazadoComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(RechazadoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
