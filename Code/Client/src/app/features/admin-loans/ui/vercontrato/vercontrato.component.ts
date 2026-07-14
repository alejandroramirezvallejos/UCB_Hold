import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  Input,
  signal,
  ViewChild,
  WritableSignal,
} from '@angular/core';
import { PrestamosAPIService } from '@entities/loan';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { extractErrorMessage } from '@shared/lib/error';
import { MostrarerrorComponent } from '@shared/ui';

const PRINT_DELAY_MS = 250;

@Component({
  selector: 'app-vercontrato',
  imports: [CommonModule, MostrarerrorComponent],
  templateUrl: './vercontrato.component.html',
  styleUrl: './vercontrato.component.css',
})
export class VercontratoComponent extends BaseTablaComponent {
  @Input() vercontraro: WritableSignal<boolean> = signal(true);
  @Input() idprestamo: number = 0;
  @ViewChild('contractContent') contractContentRef?: ElementRef<HTMLElement>;
  contratoContent: string = '';
  constructor(private readonly prestamo: PrestamosAPIService) {
    super();
  }
  ngOnInit() {
    this.cargarcontrato();
  }
  cargarcontrato() {
    this.prestamo.obtenercontratoPrestamo(this.idprestamo).subscribe({
      next: (data) => {
        if (!data || typeof data !== 'string' || data.trim() === '') {
          this.mensajeerror = 'El contrato no está disponible.';
          this.error.set(true);
          return;
        }
        this.contratoContent = data;
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'No se pudo cargar el contrato del prestamo.',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }

  descargarContrato() {
    const printContent = this.contractContentRef?.nativeElement.innerHTML;
    if (printContent) {
      const printWindow = window.open('', '_blank');
      if (printWindow) {
        printWindow.opener = null;
        printWindow.document.write(`
          <html>
            <head>
              <title>Contrato de Préstamo</title>
              <style>
                body {
                  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                  padding: 20px;
                  color: #333;
                  line-height: 1.35;
                }
                h1 {
                  text-align: center;
                  font-size: 24px;
                  color: #2c3e50;
                  margin-bottom: 12px;
                  text-transform: uppercase;
                }
                p {
                  text-align: justify;
                  margin-bottom: 10px;
                  font-size: 15px;
                }
                strong {
                  font-size: 16px;
                  color: #2c3e50;
                }
                table {
                  width: 100%;
                  border-collapse: collapse;
                  margin: 20px 0;
                  font-size: 14px;
                }
                td p {
                  margin: 0;
                  padding: 0;
                  line-height: 1.1 !important;
                  font-size: 13px;
                }
                td strong {
                  display: block;
                  margin-bottom: 2px;
                  font-size: 14px;
                }
                td {
                  border: 1px solid #aaa;
                  padding: 4px 6px;
                  text-align: left;
                  vertical-align: top;
                }
                th {
                  background-color: #e0e0e0;
                  padding: 6px 8px;
                  border: 1px solid #aaa;
                }
                .signature {
                  margin-top: 20px;
                  display: flex;
                  justify-content: center;
                  align-items: flex-start;
                  gap: 40px;
                  width: 100%;
                }
                .signature > div {
                  display: flex;
                  flex-direction: column;
                  align-items: center;
                  text-align: center;
                  max-width: 300px;
                }
                .signature img {
                  max-width: 160px;
                  height: auto;
                  border: 1px solid #ccc;
                  padding: 5px;
                  background-color: #fff;
                  margin-bottom: 5px;
                }
                .signature p {
                  margin: 0;
                  font-weight: bold;
                  text-align: center;
                }
              </style>
            </head>
            <body>
              ${printContent}
            </body>
          </html>
        `);
        printWindow.document.close();
        printWindow.setTimeout(() => {
          printWindow.print();
          printWindow.close();
        }, PRINT_DELAY_MS);
      }
    }
  }
  cerrar() {
    this.vercontraro.set(false);
  }
}
