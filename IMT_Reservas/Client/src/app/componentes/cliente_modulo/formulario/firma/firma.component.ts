import { CommonModule } from '@angular/common';
import { Component, ViewChild, ElementRef, AfterViewInit, Output, EventEmitter, Input, WritableSignal, signal } from '@angular/core';
import SignaturePad from 'signature_pad';
@Component({
  selector: 'app-firma',
  imports: [CommonModule ],
  templateUrl: './firma.component.html',
  styleUrl: './firma.component.css'
})

// TODO : REFACTORIZAR ESTO 
export class FirmaComponent {
 @ViewChild('signatureCanvas') signatureCanvas!: ElementRef<HTMLCanvasElement>;
  signaturePad!: SignaturePad;
  // Aquí se almacenará la firma en Base64 una vez guardada
  signatureData: string = '';
  @Output() firma = new EventEmitter<string>();
  @Input() clickfirma : WritableSignal<boolean> = signal(true);

  close(): void {
    this.clickfirma.set(false);
  }


  // Actualizar ngAfterViewInit
  ngAfterViewInit(): void {
    const canvas = this.signatureCanvas.nativeElement;
    this.signaturePad = new SignaturePad(canvas);
    this.resizeCanvas();
    
    // Agregar listener para resize de ventana
    window.addEventListener('resize', () => this.resizeCanvas());
  }
  
   private resizeCanvas(): void {
    const canvas = this.signatureCanvas.nativeElement;
    const ratio = Math.max(window.devicePixelRatio || 1, 1);
    
    canvas.width = canvas.offsetWidth * ratio;
    canvas.height = canvas.offsetHeight * ratio;
    canvas.getContext('2d')?.scale(ratio, ratio);
    
    this.signaturePad.clear();
  }

  clearSignature(): void {
    this.signaturePad.clear();
    this.signatureData = '';
  }

  saveSignature(): void {
    if (!this.signaturePad.isEmpty()) {
      // Convierte la firma en formato dataURL (Base64)
      this.signatureData = this.signaturePad.toDataURL();
      this.firma.emit(this.signatureData);
      this.clickfirma.set(false);

    } else {
      alert('Por favor proporciona una firma antes de guardarla.');
    }
  }
}
