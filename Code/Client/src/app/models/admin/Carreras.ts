import { Basemodel } from "../base/Basemodel";


export class Carrera extends Basemodel {
 
    Nombre?: string | null;

    constructor() {
        super();
        this.Nombre = null;
    }
}