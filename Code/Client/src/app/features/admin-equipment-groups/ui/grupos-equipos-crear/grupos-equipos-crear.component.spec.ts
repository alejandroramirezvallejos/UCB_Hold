import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { GruposEquiposCrearComponent } from './grupos-equipos-crear.component';
describe('GruposEquiposCrearComponent', () => {
  let component: GruposEquiposCrearComponent;
  let fixture: ComponentFixture<GruposEquiposCrearComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [GruposEquiposCrearComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(GruposEquiposCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
