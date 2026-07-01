import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { EquiposCrearComponent } from './equipos-crear.component';
describe('EquiposCrearComponent', () => {
  let component: EquiposCrearComponent;
  let fixture: ComponentFixture<EquiposCrearComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [EquiposCrearComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(EquiposCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
