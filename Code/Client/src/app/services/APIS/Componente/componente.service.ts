import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';




@Injectable({
  providedIn: 'root'
})
export class ComponenteService {
  private apiUrl = environment.apiUrl + '/api/Componente';

  constructor(private http : HttpClient) { }

  obetenerComponentes (){

    
  }

}
