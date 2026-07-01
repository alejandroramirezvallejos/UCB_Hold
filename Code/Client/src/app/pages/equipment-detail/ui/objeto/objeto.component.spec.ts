import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { ObjetoComponent } from './objeto.component';
describe('ObjetoComponent', () => {
  let component: ObjetoComponent;
  let fixture: ComponentFixture<ObjetoComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [ObjetoComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(ObjetoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
