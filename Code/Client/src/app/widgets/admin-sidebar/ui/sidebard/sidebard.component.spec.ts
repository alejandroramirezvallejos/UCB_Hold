import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { SidebardComponent } from './sidebard.component';
describe('SidebardComponent', () => {
  let component: SidebardComponent;
  let fixture: ComponentFixture<SidebardComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [SidebardComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(SidebardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
