import { HttpClient } from '@angular/common/http';
import { Component, OnInit ,ElementRef,ViewChild ,Renderer2, WritableSignal, signal } from '@angular/core';
import { DomSanitizer , SafeHtml  } from '@angular/platform-browser';
import {Usuario} from '../../../models/usuario'
import { FirmaComponent } from './firma/firma.component';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-formulario',
  standalone: true,
  imports: [FirmaComponent , CommonModule],
  templateUrl: './formulario.component.html',
  styleUrl: './formulario.component.css'
})

// TODO APRENDER QUE HACE
export class FormularioComponent implements OnInit {
  contenidoHtml!: SafeHtml;
  clickfirma : WritableSignal<boolean> = signal(false) 
  firma : string ="";
  
  usuario : Usuario ={
    nombre: "josue Balbontin ",
    carnet :"11111"
  }
  
  @ViewChild('contratoContainer', { static: false }) contratoContainer!: ElementRef;
  constructor(
    private http: HttpClient,
    private sanitizer: DomSanitizer,
    private renderer : Renderer2
  ) {}

  ngOnInit(): void {
    // Se hace una petición para obtener el archivo HTML desde assets
    this.http.get('assets/contrato.html', { responseType: 'text' })
      .subscribe({
        next: (data: string) => {
          const processedTemplate = this.reemplazarMarcadores(data, { 
            dia : new Date().getDate().toString() ,
            mesliteral : new Intl.DateTimeFormat('es-ES', { month: 'long' }).format(new Date()),
            año: new Date().getFullYear().toString(),
            usuario:  this.usuario.nombre!,
            usuario_ci : this.usuario.carnet!,



            firmausuario : "",

          });
          // Se sanitiza el HTML recibido
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


  guardarfirma(signatureData: string): void {
  this.firma = signatureData;
  // Actualiza la imagen de la firma en el contrato sin reprocesar todo el contenido.
  if (this.contratoContainer) {
    const signatureImage: HTMLElement | null = this.contratoContainer.nativeElement.querySelector('#firmaUsuarioPlaceholder');
    if (signatureImage) {
      this.renderer.setAttribute(signatureImage, 'src', this.firma);
    }
  }
}


  
}


