import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../../environments/environment';

// Interfaz para Accesorios
interface Accesorio {
  id: number;
  nombre: string;
  modelo: string;
  tipo: string;
  descripcion?: string;
  codigo_imt: string;
  precio: number;
  url_data_sheet?: string;
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
export class AccesoriosTablaComponent implements OnInit {
  // URL base para API
  private apiUrl = `${environment.apiUrl}/accesorios`;
  
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
  ) {
    // Inicializar formulario
    this.accesorioForm = this.fb.group({
      nombre: ['', Validators.required],
      modelo: ['', Validators.required],
      tipo: ['', Validators.required],
      descripcion: [''],
      codigo_imt: ['', Validators.required],
      precio: ['', [Validators.required, Validators.min(0)]],
      url_data_sheet: ['', Validators.pattern('https?://.+')]
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
  }
  
  cargarDatosDemostracion(): void {
    // Datos de ejemplo para demostración
    this.accesorios = [
      { id: 1, nombre: 'Cable USB', modelo: 'USB-A a USB-B', tipo: 'Cable', codigo_imt: 'ACC-001', precio: 25.00, descripcion: 'Cable para conectar dispositivos USB', url_data_sheet: 'https://example.com/usb.pdf' },
      { id: 2, nombre: 'Sensor Ultrasónico', modelo: 'HC-SR04', tipo: 'Sensor', codigo_imt: 'ACC-002', precio: 45.50, descripcion: 'Sensor de distancia por ultrasonido' },
      { id: 3, nombre: 'Jumpers M-M', modelo: 'JMM-40', tipo: 'Cable', codigo_imt: 'ACC-003', precio: 15.00, descripcion: 'Set de 40 cables jumpers macho-macho' },
      { id: 4, nombre: 'Pantalla LCD', modelo: 'LCD1602', tipo: 'Display', codigo_imt: 'ACC-004', precio: 60.00, descripcion: '16x2 caracteres con I2C', url_data_sheet: 'https://example.com/lcd.pdf' },
      { id: 5, nombre: 'Módulo Bluetooth', modelo: 'HC-05', tipo: 'Comunicaciones', codigo_imt: 'ACC-005', precio: 75.25 }
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
          accesorio.nombre.toLowerCase().includes(termino) ||
          accesorio.modelo.toLowerCase().includes(termino) ||
          accesorio.tipo.toLowerCase().includes(termino) ||
          accesorio.codigo_imt.toLowerCase().includes(termino) ||
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
      tipo: accesorio.tipo,
      descripcion: accesorio.descripcion || '',
      codigo_imt: accesorio.codigo_imt,
      precio: accesorio.precio,
      url_data_sheet: accesorio.url_data_sheet || ''
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
      return;
    }
    
    const accesorioData = this.accesorioForm.value;
    
    if (this.modalAccesorio.editar && this.modalAccesorio.id) {
      // Actualizar accesorio existente
      this.http.put(`${this.apiUrl}/${this.modalAccesorio.id}`, accesorioData).subscribe(
        () => {
          this.cargarAccesorios();
          this.cerrarModal();
        },
        (error) => {
          console.error('Error al actualizar accesorio:', error);
          // Para demostración, actualizamos localmente
          if (this.modalAccesorio.id !== undefined) {
            this.actualizarAccesorioLocal(this.modalAccesorio.id, accesorioData);
          }
          this.cerrarModal();
        }
      );
    } else {
      // Crear nuevo accesorio
      this.http.post(this.apiUrl, accesorioData).subscribe(
        (response: any) => {
          this.cargarAccesorios();
          this.cerrarModal();
        },
        (error) => {
          console.error('Error al crear accesorio:', error);
          // Para demostración, creamos localmente
          this.crearAccesorioLocal(accesorioData);
          this.cerrarModal();
        }
      );
    }
  }
  
  eliminarAccesorio(): void {
    if (!this.modalConfirmacion.id) {
      return;
    }
    
    this.http.delete(`${this.apiUrl}/${this.modalConfirmacion.id}`).subscribe(
      () => {
        this.cargarAccesorios();
        this.modalConfirmacion.visible = false;
      },
      (error) => {
        console.error('Error al eliminar accesorio:', error);
        // Para demostración, eliminamos localmente
        this.eliminarAccesorioLocal(this.modalConfirmacion.id);
        this.modalConfirmacion.visible = false;
      }
    );
  }
  
  // ====== MÉTODOS DE AYUDA PARA DATOS LOCALES (DEMOSTRACIÓN) ======
  
  actualizarAccesorioLocal(id: number, datos: any): void {
    const index = this.accesorios.findIndex(a => a.id === id);
    if (index !== -1) {
      this.accesorios[index] = { ...this.accesorios[index], ...datos };
      this.accesoriosFiltrados = [...this.accesorios];
      this.aplicarOrdenamiento();
      this.actualizarPaginacion();
    }
  }
  
  crearAccesorioLocal(datos: any): void {
    // Generar ID único para demostración
    const nuevoId = Math.max(0, ...this.accesorios.map(a => a.id)) + 1;
    const nuevoAccesorio: Accesorio = { id: nuevoId, ...datos };
    
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