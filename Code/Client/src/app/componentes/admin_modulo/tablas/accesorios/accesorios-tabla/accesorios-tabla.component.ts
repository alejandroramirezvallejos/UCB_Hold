import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../../environments/environment';

// Interfaz para Accesorios - alineada con AccesorioDto del backend
interface Accesorio {
  id: number;
  nombre?: string;
  modelo?: string;
  tipo?: string;
  descripcion?: string;
  precio?: number;
  nombreEquipoAsociado?: string;
  codigoImtEquipoAsociado?: number;
}

// Interfaz para el formulario y request al backend - actualizada para coincidir con AccesorioRequestDto
interface AccesorioRequest {
  nombre: string;
  modelo: string;
  tipo?: string | null;
  descripcion?: string | null;
  codigoIMT: number;
  precio?: number | null;
  urlDataSheet?: string | null;
}

// Estado del modal
interface ModalState {
  visible: boolean;
  editar: boolean;
  id?: number;
  nombre?: string;
}

@Component({
  selector: 'app-accesorios-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './accesorios-tabla.component.html',
  styleUrls: ['./accesorios-tabla.component.css']
})
export class AccesoriosTablaComponent implements OnInit {  // URL base para API - corregida para usar la ruta completa del controlador
  private apiUrl = `${environment.apiUrl}/api/Accesorio`;
  
  // Datos
  accesorios: Accesorio[] = [];
  accesoriosFiltrados: Accesorio[] = [];
  accesoriosPaginados: Accesorio[] = [];
  
  // Búsqueda
  terminoBusqueda: string = '';
  
  // Ordenamiento
  sortColumn: string = 'nombre';
  sortDirection: 'asc' | 'desc' = 'asc';
  
  // Paginación
  itemsPorPagina: number = 10;
  inicio: number = 0;
  paginasArray: { numero: number, activa: boolean }[] = [];
  Math = Math; // Para usar Math.min en el template
  
  // Estados de modal
  modalAccesorio: ModalState = { visible: false, editar: false };
  modalConfirmacion: ModalState = { visible: false, editar: false };
  
  // Formulario reactivo
  accesorioForm: FormGroup;
  
  constructor(
    private http: HttpClient,
    private fb: FormBuilder
  ) {    // Inicializar formulario - actualizado para backend
    this.accesorioForm = this.fb.group({
      nombre: ['', Validators.required],
      modelo: ['', Validators.required],
      tipo: [''],
      descripcion: [''],
      codigoIMT: ['', [Validators.required, Validators.min(1)]],
      precio: ['', [Validators.min(0)]],
      urlDataSheet: ['', Validators.pattern('https?://.+')]
    });
  }
  
  ngOnInit(): void {
    this.cargarAccesorios();
  }
  
  // ====== MÉTODOS PARA CARGAR DATOS ======
  
  cargarAccesorios(): void {
    this.http.get<Accesorio[]>(this.apiUrl).subscribe(
      (data) => {
        this.accesorios = data;
        this.accesoriosFiltrados = [...this.accesorios];
        this.aplicarOrdenamiento();
        this.actualizarPaginacion();
      },
      (error) => {
        console.error('Error al cargar accesorios:', error);
        // Datos de demostración
        this.cargarDatosDemostracion();
      }
    );
  }  cargarDatosDemostracion(): void {
    // Datos de ejemplo para demostración - actualizados para coincidir con AccesorioDto
    this.accesorios = [
      { id: 1, nombre: 'Cable USB', modelo: 'USB-A a USB-B', tipo: 'Cable', precio: 25.00, descripcion: 'Cable para conectar dispositivos USB', codigoImtEquipoAsociado: 1001 },
      { id: 2, nombre: 'Sensor Ultrasónico', modelo: 'HC-SR04', tipo: 'Sensor', precio: 45.50, descripcion: 'Sensor de distancia por ultrasonido', codigoImtEquipoAsociado: 1002 },
      { id: 3, nombre: 'Jumpers M-M', modelo: 'JMM-40', tipo: 'Cable', precio: 15.00, descripcion: 'Set de 40 cables jumpers macho-macho', codigoImtEquipoAsociado: 1003 },
      { id: 4, nombre: 'Pantalla LCD', modelo: 'LCD1602', tipo: 'Display', precio: 60.00, descripcion: '16x2 caracteres con I2C', codigoImtEquipoAsociado: 1004 },
      { id: 5, nombre: 'Módulo Bluetooth', modelo: 'HC-05', tipo: 'Comunicaciones', precio: 75.25, codigoImtEquipoAsociado: 1005 }
    ];
    this.accesoriosFiltrados = [...this.accesorios];
    this.aplicarOrdenamiento();
    this.actualizarPaginacion();
  }
  
  // ====== MÉTODOS DE BÚSQUEDA Y ORDENAMIENTO ======
  buscar(): void {
    if (!this.terminoBusqueda.trim()) {
      this.accesoriosFiltrados = [...this.accesorios];
    } else {
      const termino = this.terminoBusqueda.toLowerCase().trim();
      this.accesoriosFiltrados = this.accesorios.filter(
        accesorio => 
          (accesorio.nombre && accesorio.nombre.toLowerCase().includes(termino)) ||
          (accesorio.modelo && accesorio.modelo.toLowerCase().includes(termino)) ||
          (accesorio.tipo && accesorio.tipo.toLowerCase().includes(termino)) ||
          (accesorio.codigoImtEquipoAsociado && accesorio.codigoImtEquipoAsociado.toString().includes(termino)) ||
          (accesorio.descripcion && accesorio.descripcion.toLowerCase().includes(termino))
      );
    }
    
    // Reiniciar paginación al buscar
    this.inicio = 0;
    this.aplicarOrdenamiento();
    this.actualizarPaginacion();
  }
  
  limpiarBusqueda(): void {
    this.terminoBusqueda = '';
    this.accesoriosFiltrados = [...this.accesorios];
    this.inicio = 0;
    this.aplicarOrdenamiento();
    this.actualizarPaginacion();
  }
  
  ordenarPor(columna: string): void {
    if (this.sortColumn === columna) {
      // Cambiar dirección si es la misma columna
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      // Nueva columna, ordenar ascendente
      this.sortColumn = columna;
      this.sortDirection = 'asc';
    }
    
    this.aplicarOrdenamiento();
    this.actualizarPaginacion();
  }
  
  aplicarOrdenamiento(): void {
    this.accesoriosFiltrados.sort((a: any, b: any) => {
      let valorA = a[this.sortColumn];
      let valorB = b[this.sortColumn];
      
      // Manejar caso insensible para strings
      if (typeof valorA === 'string') {
        valorA = valorA.toLowerCase();
        valorB = valorB.toLowerCase();
      }
      
      if (valorA < valorB) {
        return this.sortDirection === 'asc' ? -1 : 1;
      }
      if (valorA > valorB) {
        return this.sortDirection === 'asc' ? 1 : -1;
      }
      return 0;
    });
  }
  
  // ====== MÉTODOS DE PAGINACIÓN ======
  
  cambiarPagina(nuevoInicio: number): void {
    if (nuevoInicio < 0 || nuevoInicio >= this.accesoriosFiltrados.length) {
      return;
    }
    
    this.inicio = nuevoInicio;
    this.actualizarPaginacion();
  }
  
  actualizarPaginacion(): void {
    // Actualizar datos paginados
    this.accesoriosPaginados = this.accesoriosFiltrados.slice(
      this.inicio,
      Math.min(this.inicio + this.itemsPorPagina, this.accesoriosFiltrados.length)
    );
    
    // Calcular páginas para mostrar
    const totalPaginas = Math.ceil(this.accesoriosFiltrados.length / this.itemsPorPagina);
    const paginaActual = Math.floor(this.inicio / this.itemsPorPagina) + 1;
    
    this.paginasArray = [];
    
    // Mostrar máximo 5 páginas
    let inicio = Math.max(1, paginaActual - 2);
    let fin = Math.min(totalPaginas, inicio + 4);
    
    // Ajustar inicio si estamos cerca del final
    if (fin === totalPaginas) {
      inicio = Math.max(1, fin - 4);
    }
    
    for (let i = inicio; i <= fin; i++) {
      this.paginasArray.push({
        numero: i,
        activa: i === paginaActual
      });
    }
  }
  
  // ====== MÉTODOS PARA GESTIÓN DE MODALES ======
  
  abrirModalCrear(): void {
    this.accesorioForm.reset();
    this.modalAccesorio = { visible: true, editar: false };
  }
    abrirModalEditar(accesorio: Accesorio): void {
    this.accesorioForm.patchValue({
      nombre: accesorio.nombre,
      modelo: accesorio.modelo,
      tipo: accesorio.tipo || '',
      descripcion: accesorio.descripcion || '',
      codigoIMT: accesorio.codigoImtEquipoAsociado || '',
      precio: accesorio.precio || '',
      urlDataSheet: ''
    });
    
    this.modalAccesorio = { 
      visible: true, 
      editar: true,
      id: accesorio.id
    };
  }
  
  cerrarModal(): void {
    this.modalAccesorio.visible = false;
  }
  
  confirmarEliminar(accesorio: Accesorio, event: Event): void {
    // Evitar que se propague y abra el modal de edición
    event.stopPropagation();
    
    this.modalConfirmacion = {
      visible: true,
      editar: false,
      id: accesorio.id,
      nombre: accesorio.nombre
    };
  }
  
  cancelarEliminacion(): void {
    this.modalConfirmacion.visible = false;
  }
    // ====== MÉTODOS PARA OPERACIONES CRUD ======
    guardarAccesorio(): void {
    if (this.accesorioForm.invalid) {
      console.error('Formulario inválido:', this.accesorioForm.errors);
      this.marcarCamposComoTocados();
      return;
    }
    
    const formValue = this.accesorioForm.value;
    console.log('Valores del formulario:', formValue);
      // Validar campos requeridos manualmente
    if (!formValue.nombre || !formValue.modelo || !formValue.codigoIMT) {
      console.error('Campos requeridos faltantes:', {
        nombre: formValue.nombre,
        modelo: formValue.modelo,
        codigoIMT: formValue.codigoIMT
      });
      alert('Por favor, complete todos los campos requeridos: Nombre, Modelo y Código IMT');
      this.marcarCamposComoTocados();
      return;
    }

    // Validar que CodigoIMT sea un número positivo
    const codigoIMT = parseInt(formValue.codigoIMT);
    if (isNaN(codigoIMT) || codigoIMT <= 0) {
      alert('El Código IMT debe ser un número natural (mayor a 0)');
      this.marcarCamposComoTocados();
      return;
    }    // Asegurar que el dato del backend coincida con el DTO esperado
    const accesorioData: AccesorioRequest = {
      nombre: formValue.nombre.trim(),
      modelo: formValue.modelo.trim(),
      tipo: formValue.tipo?.trim() || null,
      descripcion: formValue.descripcion?.trim() || null,
      codigoIMT: codigoIMT,
      precio: formValue.precio ? parseFloat(formValue.precio) : null,
      urlDataSheet: formValue.urlDataSheet?.trim() || null
    };
    
    console.log('Datos a enviar al backend:', accesorioData);
    
    if (this.modalAccesorio.editar && this.modalAccesorio.id) {
      // Actualizar accesorio existente
      console.log(`Actualizando accesorio con ID: ${this.modalAccesorio.id}`);
      this.http.put(`${this.apiUrl}/${this.modalAccesorio.id}`, accesorioData).subscribe(
        (response) => {
          console.log('Accesorio actualizado exitosamente:', response);
          this.cargarAccesorios();
          this.cerrarModal();
        },
        (error) => {
          console.error('Error al actualizar accesorio:', error);
          console.error('Detalles del error:', error.error);
          alert(`Error al actualizar: ${error.error?.message || error.message}`);
        }
      );
    } else {
      // Crear nuevo accesorio
      console.log('Creando nuevo accesorio');
      this.http.post(this.apiUrl, accesorioData).subscribe(
        (response: any) => {
          console.log('Accesorio creado exitosamente:', response);
          this.cargarAccesorios();
          this.cerrarModal();
        },
        (error) => {
          console.error('Error al crear accesorio:', error);
          console.error('Detalles del error:', error.error);
          alert(`Error al crear: ${error.error?.message || error.message}`);
        }
      );
    }
  }

  // Método auxiliar para marcar todos los campos como tocados y mostrar errores
  private marcarCamposComoTocados(): void {
    Object.keys(this.accesorioForm.controls).forEach(key => {
      this.accesorioForm.get(key)?.markAsTouched();
    });
  }
    eliminarAccesorio(): void {
    if (!this.modalConfirmacion.id) {
      console.error('No hay ID para eliminar');
      return;
    }
    
    console.log(`Eliminando accesorio con ID: ${this.modalConfirmacion.id}`);
    
    this.http.delete(`${this.apiUrl}/${this.modalConfirmacion.id}`).subscribe(
      (response) => {
        console.log('Accesorio eliminado exitosamente:', response);
        this.cargarAccesorios();
        this.modalConfirmacion.visible = false;
      },
      (error) => {
        console.error('Error al eliminar accesorio:', error);
        console.error('Detalles del error:', error.error);
        alert(`Error al eliminar: ${error.error?.message || error.message}`);
        this.modalConfirmacion.visible = false;
      }
    );
  }
    // ====== MÉTODOS DE AYUDA PARA DATOS LOCALES (DEMOSTRACIÓN) ======
  actualizarAccesorioLocal(id: number, datos: AccesorioRequest): void {
    const index = this.accesorios.findIndex(a => a.id === id);
    if (index !== -1) {
      // Convertir AccesorioRequest a Accesorio para actualización local
      this.accesorios[index] = { 
        ...this.accesorios[index], 
        nombre: datos.nombre,
        modelo: datos.modelo,
        tipo: datos.tipo || undefined,
        descripcion: datos.descripcion || undefined,
        precio: datos.precio || undefined,
        codigoImtEquipoAsociado: datos.codigoIMT
      };
      this.accesoriosFiltrados = [...this.accesorios];
      this.aplicarOrdenamiento();
      this.actualizarPaginacion();
    }
  }
  
  crearAccesorioLocal(datos: AccesorioRequest): void {
    // Generar ID único para demostración
    const nuevoId = Math.max(0, ...this.accesorios.map(a => a.id)) + 1;
    const nuevoAccesorio: Accesorio = { 
      id: nuevoId, 
      nombre: datos.nombre,
      modelo: datos.modelo,
      tipo: datos.tipo || undefined,
      descripcion: datos.descripcion || undefined,
      precio: datos.precio || undefined,
      codigoImtEquipoAsociado: datos.codigoIMT
    };
    
    this.accesorios.push(nuevoAccesorio);
    this.accesoriosFiltrados = [...this.accesorios];
    this.aplicarOrdenamiento();
    this.actualizarPaginacion();
  }
  
  eliminarAccesorioLocal(id?: number): void {
    if (!id) return;
    
    this.accesorios = this.accesorios.filter(a => a.id !== id);
    this.accesoriosFiltrados = [...this.accesorios];
    this.aplicarOrdenamiento();
    this.actualizarPaginacion();
  }
}