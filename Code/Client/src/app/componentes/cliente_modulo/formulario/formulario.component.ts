import { HttpClient } from '@angular/common/http';
import { Component, OnInit ,ElementRef,ViewChild ,Renderer2, WritableSignal, signal } from '@angular/core';
import { DomSanitizer , SafeHtml  } from '@angular/platform-browser';
import { FirmaComponent } from './firma/firma.component';
import { CommonModule } from '@angular/common';
import { CarritoService } from '../../../services/carrito/carrito.service';
import {Carrito} from '../../../models/carrito'
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { MostrarerrorComponent } from '../../mostrarerror/mostrarerror.component';
import { Router } from '@angular/router';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../services/APIS/prestamo/prestamos-api.service';
import { PantallaCargaComponent } from '../../pantalla-carga/pantalla-carga.component';
import { finalize } from 'rxjs';



@Component({
  selector: 'app-formulario',
  standalone: true,
  imports: [FirmaComponent , CommonModule , MostrarerrorComponent,PantallaCargaComponent],
  templateUrl: './formulario.component.html',
  styleUrl: './formulario.component.css'
})


export class FormularioComponent implements OnInit {
  @ViewChild('contratoContainer', { static: false }) contratoContainer!: ElementRef;
  contenidoHtml!: SafeHtml;
  clickfirma : WritableSignal<boolean> = signal(false) 
  firma : string ="";
  error : WritableSignal<number> = signal(2); 
  mensajeerror : string = "Error desconocido intente mas tarde";
  cargando : boolean = false;

  
 
  constructor(
    private http: HttpClient,
    private sanitizer: DomSanitizer,
    private renderer : Renderer2,
    private carrito : CarritoService,
    private router : Router,
    private usuario : UsuarioService,
    private mandarprestamo : PrestamosAPIService
  ) {

  }

  ngOnInit(): void {

    const fechaInicio = new Date(this.carrito.obtenerfechainicio());
    const fechaFinal = new Date(this.carrito.obtenerfechafinal());

    const diffDias = Math.ceil((fechaFinal.getTime() - fechaInicio.getTime()) / (1000 * 60 * 60 * 24));

    this.http.get('assets/contrato.html', { responseType: 'text' })
      .subscribe({
        next: (data: string) => {
          const processedTemplate = this.reemplazarMarcadores(data, { 
            dia : new Date().getDate().toString() ,
            mesliteral : new Intl.DateTimeFormat('es-ES', { month: 'long' }).format(new Date()),
            año: new Date().getFullYear().toString(),
            usuario:  this.usuario.usuario.nombre!,
            usuario_ci : this.usuario.usuario.carnet!,
            tablaprimera: this.primeradelobjeto(this.carrito.obtenercarrito()),
            fechaMaxima : String(diffDias),
            precio : this.carrito.preciototal().toString(),
            tablasegunda : this.quintavalordebienes(this.carrito.obtenercarrito()),
            dia_devolucion : (fechaFinal.getDate()+1).toString(),
            mes_devolucion : (fechaFinal.getMonth()+1).toString(),
            año_devolucion : fechaFinal.getFullYear().toString(),
            firmausuario : "",

          });
          this.contenidoHtml = this.sanitizer.bypassSecurityTrustHtml(processedTemplate);
        },
        error: (error) => console.error('Error al cargar el HTML: ', error)
      });
  }

 private reemplazarMarcadores(template: string, valores: { [clave: string]: string }): string {
    let resultado = template;
    for (const clave in valores) {
      const regex = new RegExp(`\\[\\[${clave}\\]\\]`, 'g');
      resultado = resultado.replace(regex, valores[clave]);
    }
    return resultado;
  }

  firmar(){

    this.clickfirma.set(true);

  }  
  
  aceptar(){
    if(!this.carrito || Object.keys(this.carrito.obtenercarrito()).length===0){
      this.error.set(1); 
      this.mensajeerror = "El carrito está vacío. Agregue elementos antes de continuar.";
    }
    else if(!this.firma || this.firma === ''){
      this.firmar();
    } 
    else{
      const contratoblob= this.generarHTMLBinario(); 
      
      this.cargando = true;
      this.mandarprestamo.crearPrestamo(this.carrito.obtenercarrito(),this.usuario.usuario.carnet!,contratoblob)
      .pipe(finalize(() => this.cargando = false))
      .subscribe({
        next: (response) => {
          console.log('Préstamo creado exitosamente:', response);
          alert('Préstamo creado exitosamente');
          this.carrito.vaciarcarrito();

          this.router.navigate(["/home"]);
        },
        error: (error) => {
        
          console.error('Error al crear préstamo:', error);
          this.error.set(1);
          this.mensajeerror = error.error.error+ " - " + error.error.mensaje;
        }
      })
    }
  }
  
  
  guardarfirma(signatureData: string): void {
  this.firma = signatureData;
 
  if (this.contratoContainer) {
    const signatureImage: HTMLElement | null = this.contratoContainer.nativeElement.querySelector('#firmaUsuarioPlaceholder');
    if (signatureImage) {
      this.renderer.setAttribute(signatureImage, 'src', this.firma);
    }
  }
  }




  private formatearCodigos(codigos?: string[]): string {
  return (codigos ?? []).join(', ') || 'Por definirse';
  }

private primeradelobjeto(carrito: Carrito): string {
   const items = Object.values(carrito)
    .filter(item => typeof item === 'object' && 'nombre' in item);

  return `
    ${items.map((item, index) => `
      <tr>
        <td>${this.formatearCodigos(item.codigo_ucb_unico)}</td>
        <td>
        <strong>${item.nombre}</strong>
        <p>Marca: ${item.marca} </p>
        <p>Modelo: ${item.modelo} </p>
        </td>
        <td>${this.formatearCodigos(item.numero_serie_unico)}</td>
        <td>${item.cantidad}</td> 
      </tr>
    `).join('')}
  `;
}

private quintavalordebienes(carrito :  Carrito) : string{
 const items = Object.values(carrito)
    .filter(item => typeof item === 'object' && 'nombre' in item);

  return `
    ${items.map((item, index) => `
      <tr>
        <td>${this.formatearCodigos(item.codigo_ucb_unico)}</td>
        <td>
        <strong>${item.nombre}</strong>
        <p>Marca: ${item.marca} </p>
        <p>Modelo: ${item.modelo} </p>
        </td>
        <td>${this.formatearCodigos(item.numero_serie_unico)}</td>
        <td>${item.cantidad}</td>
        <td>${item.precio}</td>
        <td>${item.precio * item.cantidad}</td> 
      </tr>
    `).join('')}
  `;

}
    


generarHTMLBinario(): Blob {
  // Obtener el HTML actualizado del contenedor del contrato
  const contratoElement = this.contratoContainer.nativeElement;
  const htmlContent = contratoElement.outerHTML;
  
  // Crear un Blob con el contenido HTML
  const htmlBlob = new Blob([htmlContent], { type: 'text/html;charset=utf-8' });
  
  return htmlBlob;
}

}


