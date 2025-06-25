import { Basemodel } from "../base/Basemodel";

export class Componente extends Basemodel {
  Nombre?: string | null;
  Modelo?: string | null;
  Tipo?: string | null;
  Descripcion?: string | null;
  PrecioReferencia?: number | null;
  NombreEquipo?: string | null;
  CodigoImtEquipo?: number | null;
  UrlDataSheet?: string | null;

  constructor(){
    super();
    this.Nombre = null;
    this.Modelo = null;
    this.Tipo = null;
    this.Descripcion = null;
    this.PrecioReferencia = null;
    this.NombreEquipo = null;
    this.CodigoImtEquipo = null;
    this.UrlDataSheet = null;
  }


}