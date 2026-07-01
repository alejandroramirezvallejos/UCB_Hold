import { AfterViewInit, Directive, ElementRef, OnDestroy } from '@angular/core';

@Directive({
  selector: '[appStickyScroll]',
  standalone: true,
})
export class StickyScrollDirective implements AfterViewInit, OnDestroy {
  private phantom!: HTMLDivElement;
  private inner!: HTMLDivElement;
  private style!: HTMLStyleElement;
  private syncing = false;
  private listeners: Array<() => void> = [];

  constructor(private readonly el: ElementRef<HTMLElement>) {}

  ngAfterViewInit(): void {
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

    this.style = document.createElement('style');
    this.style.textContent = `
      .sticky-scroll-phantom { scrollbar-color: rgba(148, 163, 184, 0.65) transparent; scrollbar-width: thin; }
      .sticky-scroll-phantom::-webkit-scrollbar { height: 8px; }
      .sticky-scroll-phantom::-webkit-scrollbar-thumb { background: rgba(148, 163, 184, 0.65); border: 2px solid transparent; border-radius: 999px; background-clip: content-box; }
      .sticky-scroll-phantom::-webkit-scrollbar-thumb:hover { background: rgba(100, 116, 139, 0.85); border: 2px solid transparent; background-clip: content-box; }
      .sticky-scroll-phantom::-webkit-scrollbar-track { background: rgba(226, 232, 240, 0.35); }
    `;
    document.head.appendChild(this.style);
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

  ngOnDestroy(): void {
    this.listeners.forEach((fn) => fn());
    this.phantom?.remove();
    this.style?.remove();
  }
}
