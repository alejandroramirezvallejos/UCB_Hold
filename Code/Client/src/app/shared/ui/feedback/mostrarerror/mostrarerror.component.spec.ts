import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { MostrarerrorComponent } from './mostrarerror.component';
describe('MostrarerrorComponent', () => {
  let component: MostrarerrorComponent;
  let fixture: ComponentFixture<MostrarerrorComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [MostrarerrorComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(MostrarerrorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
