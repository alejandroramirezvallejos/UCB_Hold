import { Basemodel } from '@shared/model';
export class Carrera extends Basemodel {
  Nombre?: string | null;
  constructor() {
    super();
    this.Nombre = null;
  }
}
