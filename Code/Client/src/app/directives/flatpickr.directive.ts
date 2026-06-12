import { Directive, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import flatpickr from 'flatpickr';
import { Spanish } from 'flatpickr/dist/l10n/es';
import { Options } from 'flatpickr/dist/types/options';

@Directive({
  selector: '[appFlatpickr]',
  standalone: true
})
export class FlatpickrDirective implements OnInit, OnDestroy {
  @Input() fpOptions: Partial<Options> = {};
  @Output() fpChange = new EventEmitter<Date[]>();
  @Output() fpReady = new EventEmitter<flatpickr.Instance>();

  private instance?: flatpickr.Instance;

  constructor(private el: ElementRef<HTMLInputElement>) {}

  ngOnInit() {
    this.instance = flatpickr(this.el.nativeElement, {
      locale: Spanish,
      dateFormat: 'Y-m-d',
      allowInput: false,
      disableMobile: true,
      monthSelectorType: 'static',
      ...this.fpOptions,
      onChange: (dates, dateStr) => {
        this.fpChange.emit(dates);
        // Keep ngModel in sync
        this.el.nativeElement.dispatchEvent(new Event('input'));
        this.el.nativeElement.dispatchEvent(new Event('change'));
      },
      onReady: (_dates, _str, instance) => {
        const yearInput = instance.currentYearElement;
        if (yearInput) {
          yearInput.readOnly = true;
          yearInput.tabIndex = -1;
          yearInput.addEventListener('wheel', (e) => e.preventDefault(), { passive: false });
          yearInput.addEventListener('keydown', (e) => e.preventDefault());
        }
        this.fpReady.emit(instance);
      },
    }) as flatpickr.Instance;
  }

  ngOnDestroy() {
    this.instance?.destroy();
  }
}
