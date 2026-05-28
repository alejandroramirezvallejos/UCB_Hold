import { CommonModule } from '@angular/common';
import { Component, Input, signal, WritableSignal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GrupoequipoService } from '../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { GrupoEquipo } from '../../../models/grupo_equipo';
import { CarritoService } from '../../../services/carrito/carrito.service';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { DisponibilidadService } from '../../../services/APIS/Disponibilidad/disponibilidad.service';
import { extractErrorMessage } from '../../../utils/error-handler';
@Component({
  selector: 'app-objeto',
  standalone: true,
  imports: [CommonModule , MostrarerrorComponent],
  templateUrl: './objeto.component.html',
  styleUrl: './objeto.component.css'
})
export class ObjetoComponent {
  @Input() id: string = ''
  producto: GrupoEquipo = new GrupoEquipo();
  cantidadDisponible: number = 0;
  totalOperativo: number = 0;
  cargando: boolean = true;
  addedToCart = false;
  error : WritableSignal<boolean> = signal(false);
  mensajeerror : string = "";
  desabilitarboton: boolean = false;
  sinUnidadesOperativas: boolean = false;
  constructor(private route: ActivatedRoute , private servicio : GrupoequipoService, private carrito : CarritoService, private SDisponibilidad : DisponibilidadService) { }
  ngOnInit(): void {
    const routeId = this.route.snapshot.paramMap.get('id');
    if (!routeId) {
      console.error('ID no proporcionado en la URL');
      return;
    }
    this.id = routeId;
    this.servicio.getproducto(routeId).subscribe({
      next: (data) => {
        this.producto = data;
        if (this.producto && this.producto.id) {
            this.obtenerDisponibilidad();
        } else {
            this.cargando = false;
        }
      },
      error: (error) => {
        this.desabilitarboton = true;
        const errorMsg = extractErrorMessage(error, 'Error al cargar el producto, intente mas tarde');
        this.mensajeerror = errorMsg;
        console.error('Error completo del backend:', errorMsg);
        this.producto = {
          id: 0,
          nombre: 'Error de carga',
          descripcion: 'No se pudo cargar la información del producto. Intente más tarde.',
          modelo: '',
          marca: '',
          url_data_sheet: '',
          link: ''
        };
        this.cargando = false;
        this.error.set(true);
      }
    });
  }
  obtenerDisponibilidad(){
    this.SDisponibilidad.obtenerDisponibilidad(new Date(), new Date() , [this.producto.id]).subscribe({
      next: (data) => {
        if(data && data.length > 0){
          this.cantidadDisponible = data[0].CantidadDisponible;
          this.totalOperativo = data[0].TotalOperativo ?? 0;
        } else {
          this.cantidadDisponible = 0;
          this.totalOperativo = 0;
        }
        if (this.totalOperativo === 0) {
          this.sinUnidadesOperativas = true;
          this.desabilitarboton = true;
        }
        this.cargando = false;
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, 'Error al obtener la disponibilidad del producto');
        console.error('Error disponibilidad:', errorMsg);
        this.mensajeerror = errorMsg;
        this.error.set(true);
        this.cargando = false;
      }
    });
  }
  addproductocarrito() {
    if (this.addedToCart) {
      return;
    }
    this.carrito.agregarproducto(
      this.producto.id,
      this.producto.nombre,
      this.producto.link ?? '',
      this.producto.marca ?? '',
      this.producto.modelo ?? '',
      this.producto.CostoPromedio ?? 0,
      this.producto.Cantidad ?? 1
    );
    this.addedToCart = true;
  }
}
