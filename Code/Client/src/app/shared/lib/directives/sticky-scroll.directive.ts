import { AfterViewInit, Directive, ElementRef, OnDestroy } from '@angular/core';

@Directive({
  selector: '[appStickyScroll]',
  standalone: true,
})
export class StickyScrollDirective implements AfterViewInit, OnDestroy {
  private phantom!: HTMLDivElement;
  private inner!: HTMLDivElement;
  private syncing = false;
  private listeners: Array<() => void> = [];

  constructor(private readonly el: ElementRef<HTMLElement>) {}

  ngAfterViewInit() {
    const host = this.el.nativeElement;

    this.phantom = document.createElement('div');
    Object.assign(this.phantom.style, {
      position: 'fixed',
      bottom: '0',
      height: '10px',
      overflowX: 'auto',
      overflowY: 'hidden',
      zIndex: '998',
      display: 'none',
      background: 'transparent',
      cursor: 'default',
    });

    const style = document.createElement('style');
    style.textContent = `
      .sticky-scroll-phantom::-webkit-scrollbar { height: 6px; }
      .sticky-scroll-phantom::-webkit-scrollbar-thumb { background: rgba(0,0,0,0.2); border-radius: 999px; }
      .sticky-scroll-phantom::-webkit-scrollbar-track { background: transparent; }
    `;
    document.head.appendChild(style);
    this.phantom.classList.add('sticky-scroll-phantom');

    this.inner = document.createElement('div');
    this.inner.style.height = '1px';
    this.phantom.appendChild(this.inner);
    document.body.appendChild(this.phantom);

    const syncToHost = () => {
      if (this.syncing) return;
      this.syncing = true;
      host.scrollLeft = this.phantom.scrollLeft;
      this.syncing = false;
    };
    const syncToPhantom = () => {
      if (this.syncing) return;
      this.syncing = true;
      this.phantom.scrollLeft = host.scrollLeft;
      this.syncing = false;
    };
    const update = () => {
      const rect = host.getBoundingClientRect();
      const needsScroll = host.scrollWidth > host.clientWidth + 2;
      const isVisible =
        rect.bottom > 0 && rect.top < window.innerHeight && needsScroll;
      if (isVisible) {
        this.phantom.style.display = 'block';
        this.inner.style.width = host.scrollWidth + 'px';
        this.phantom.style.left = rect.left + 'px';
        this.phantom.style.width = rect.width + 'px';
      } else {
        this.phantom.style.display = 'none';
      }
    };

    this.phantom.addEventListener('scroll', syncToHost);
    host.addEventListener('scroll', syncToPhantom);
    window.addEventListener('scroll', update, { passive: true });
    window.addEventListener('resize', update, { passive: true });

    this.listeners = [
      () => this.phantom.removeEventListener('scroll', syncToHost),
      () => host.removeEventListener('scroll', syncToPhantom),
      () => window.removeEventListener('scroll', update),
      () => window.removeEventListener('resize', update),
    ];

    setTimeout(update, 100);
  }

  ngOnDestroy() {
    this.listeners.forEach((fn) => fn());
    this.phantom?.remove();
  }
}
