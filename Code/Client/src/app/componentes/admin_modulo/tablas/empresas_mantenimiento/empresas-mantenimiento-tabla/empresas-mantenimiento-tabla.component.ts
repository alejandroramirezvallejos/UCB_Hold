import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmpresaMantenimiento } from '../../../../../models/admin/EmpresaMantenimiento';
import { EmpresasMantenimientoCrearComponent } from '../empresas-mantenimiento-crear/empresas-mantenimiento-crear.component';
import { EmpresasMantenimientoEditarComponent } from '../empresas-mantenimiento-editar/empresas-mantenimiento-editar.component';
import { EmpresamantenimientoService } from '../../../../../services/APIS/EmpresaMantenimiento/empresamantenimiento.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-empresas-mantenimiento-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, EmpresasMantenimientoCrearComponent, EmpresasMantenimientoEditarComponent, AvisoEliminarComponent , MostrarerrorComponent, AvisoExitoComponent],
  templateUrl: './empresas-mantenimiento-tabla.component.html',
  styleUrl: './empresas-mantenimiento-tabla.component.css'
})
export class EmpresasMantenimientoTablaComponent extends BaseTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  empresas: EmpresaMantenimiento[] = [];
  empresascopia: EmpresaMantenimiento[] = [];

  empresaSeleccionada: EmpresaMantenimiento = new EmpresaMantenimiento();

  terminoBusqueda: string = '';

 

  constructor(private empresaService: EmpresamantenimientoService) {
    super(); 
  }

  ngOnInit() {
    this.cargarEmpresas();
  }

  limpiarEmpresaSeleccionada() {
    this.empresaSeleccionada = new EmpresaMantenimiento();
  }  
  crearempresamantenimiento() {
    this.botoncrear.set(true);
  }

  cargarEmpresas() {
    this.empresaService.obtenerEmpresaMantenimiento().subscribe(
      (data: EmpresaMantenimiento[]) => {
        this.empresas = data;
        this.empresascopia = [...this.empresas];
      },
      (error) => {
        this.mensajeerror = 'Error al cargar las empresas de mantenimiento, intente más tarde.';
        console.error('Error al cargar las empresas de mantenimiento:', error);
        this.error.set(true);
      }
    );
  }
  private normalizeText(text: string): string {
    return text
      .toLowerCase()
      .normalize('NFD')  // Descompone caracteres con acentos
      .replace(/[\u0300-\u036f]/g, '');  // Elimina diacríticos
  }
  
  buscar() {
    if (this.terminoBusqueda.trim() === '') {
      this.limpiarBusqueda();
      return;
    }

    const busquedaNormalizada = this.normalizeText(this.terminoBusqueda);

    this.empresas = this.empresascopia.filter(empresa =>
      this.normalizeText(empresa.NombreEmpresa || '').includes(busquedaNormalizada) ||
      this.normalizeText(empresa.Telefono || '').includes(busquedaNormalizada) ||
      this.normalizeText(empresa.Nit || '').includes(busquedaNormalizada)
    );
  }

  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.empresas = [...this.empresascopia];
  }
  editarEmpresaMantenimiento(empresa: EmpresaMantenimiento) {
    this.botoncrear.set(false);
    this.empresaSeleccionada = { ...empresa };
    this.botoneditar.set(true);
  }
  eliminarEmpresaMantenimiento(empresa: EmpresaMantenimiento) {
    this.empresaSeleccionada = empresa;
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
    if (this.empresaSeleccionada.Id) {
      this.empresaService.eliminarEmpresaMantenimiento(this.empresaSeleccionada.Id).subscribe({
        next : (response) => {
          this.cargarEmpresas();
          this.mensajeexito = 'Empresa de mantenimiento eliminada exitosamente.';
          this.exito.set(true);
        },
        error: (error) => {
          this.mensajeerror = 'Error al eliminar la empresa de mantenimiento.';
          console.error('Error al eliminar la empresa de mantenimiento: ' + error);
          this.error.set(true);
        }
      });
    }
    this.limpiarEmpresaSeleccionada();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarEmpresaSeleccionada();
  }


}
