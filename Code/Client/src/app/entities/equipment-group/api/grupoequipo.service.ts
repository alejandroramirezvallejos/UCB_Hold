import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable, of, tap } from 'rxjs';
import { GrupoEquipo } from '../model/grupo_equipo';
import { ApiResponse, extractApiValue } from '@shared/api';
import { GrupoEquipoApiItem } from './grupo-equipo-api-item';
@Injectable({
  providedIn: 'root',
})
export class GrupoequipoService {
  private readonly apiUrl = environment.apiUrl + '/api/GrupoEquipo';
  private readonly grupoEquipoApiVacio: GrupoEquipoApiItem = {
    Id: 0,
    Nombre: null,
  };
  private _cache: GrupoEquipo[] | null = null;
  paginaGuardada: number = 0;
  cantidadObjetosGuardada: number = 21;
  constructor(private readonly http: HttpClient) {}

  invalidarCache() {
    this._cache = null;
  }

  private mapearGrupoEquipo(item: GrupoEquipoApiItem): GrupoEquipo {
    return {
      id: item.Id,
      nombre: item.Nombre,
      descripcion: item.Descripcion || '',
      modelo: item.Modelo ? ` ${item.Modelo}` : '',
      url_data_sheet: item.UrlDataSheet || '',
      marca: item.Marca ? ` ${item.Marca}` : '',
      link: item.UrlImagen ?? null,
      nombreCategoria: item.NombreCategoria || '',
      Cantidad: item.Cantidad || 0,
      CostoPromedio: item.CostoPromedio || 0,
    };
  }

  crearGrupoEquipo(grupoEquipo: GrupoEquipo) {
    const envio = {
      Nombre: grupoEquipo.nombre,
      Modelo: grupoEquipo.modelo,
      Marca: grupoEquipo.marca,
      NombreCategoria: grupoEquipo.nombreCategoria,
      Descripcion: grupoEquipo.descripcion,
      UrlDataSheet: grupoEquipo.url_data_sheet,
      UrlImagen: grupoEquipo.link,
    };
    return this.http
      .post<unknown>(this.apiUrl, envio)
      .pipe(tap(() => this.invalidarCache()));
  }

  obtenersinfiltroGruposEquipos(): Observable<GrupoEquipo[]> {
    return this.http.get<ApiResponse<GrupoEquipoApiItem[]>>(this.apiUrl).pipe(
      map((data) =>
        extractApiValue(data, []).map((item) => ({
          ...this.mapearGrupoEquipo(item),
          modelo: item.Modelo,
          marca: item.Marca,
          descripcion: item.Descripcion,
          url_data_sheet: item.UrlDataSheet,
        })),
      ),
    );
  }

  getGrupoEquipo(
    categoria: string,
    producto: string,
  ): Observable<GrupoEquipo[]> {
    if (this._cache !== null) return of(this._cache);
    const url =
      this.apiUrl + '/buscar?nombre=' + producto + '&categoria=' + categoria;
    return this.http.get<ApiResponse<GrupoEquipoApiItem[]>>(url).pipe(
      map((data) =>
        extractApiValue(data, []).map((item) => this.mapearGrupoEquipo(item)),
      ),
      tap((result) => {
        this._cache = result;
      }),
    );
  }

  getproducto(id: string): Observable<GrupoEquipo> {
    const url = `${this.apiUrl}/${id}`;
    return this.http
      .get<ApiResponse<GrupoEquipoApiItem>>(url)
      .pipe(
        map((data) =>
          this.mapearGrupoEquipo(
            extractApiValue(data, this.grupoEquipoApiVacio),
          ),
        ),
      );
  }

  editarGrupoEquipo(grupoEquipo: GrupoEquipo) {
    const envio = {
      Id: grupoEquipo.id,
      Nombre: grupoEquipo.nombre,
      Modelo: grupoEquipo.modelo,
      Marca: grupoEquipo.marca,
      NombreCategoria: grupoEquipo.nombreCategoria,
      Descripcion: grupoEquipo.descripcion,
      UrlDataSheet: grupoEquipo.url_data_sheet,
      UrlImagen: grupoEquipo.link,
    };
    return this.http
      .put<unknown>(`${this.apiUrl}/${grupoEquipo.id}`, envio)
      .pipe(tap(() => this.invalidarCache()));
  }

  eliminarGrupoEquipo(id: number) {
    return this.http
      .delete<unknown>(`${this.apiUrl}/${id}`)
      .pipe(tap(() => this.invalidarCache()));
  }
}
