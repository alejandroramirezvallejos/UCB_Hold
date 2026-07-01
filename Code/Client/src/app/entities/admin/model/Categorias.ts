import { Basemodel } from '@shared/model';
export class Categorias extends Basemodel {
  Nombre?: string | null;
  constructor() {
    super();
    this.Nombre = null;
  }
}
