namespace IMT_Reservas.Tests.ControllerTests
{
    public interface IPrestamoControllerTest
    {
        void GetPrestamos_ConDatos_RetornaOk();
        void GetPrestamos_SinDatos_RetornaOkVacia();
        void GetPrestamos_ServicioError_RetornaError500();
        void CrearPrestamo_Valido_RetornaOk();
        void CrearPrestamo_Invalido_RetornaBadRequest(CrearPrestamoComando comando, System.Exception excepcionLanzada);
        void CrearPrestamo_UsuarioNoEncontrado_RetornaNotFound();
        void CrearPrestamo_SinEquiposDisponibles_RetornaConflict();
        void EliminarPrestamo_Valido_RetornaOk();
        void EliminarPrestamo_NoEncontrado_RetornaNotFound();
        void EliminarPrestamo_EnUso_RetornaConflict();
        void AceptarPrestamo_Valido_RetornaOk();
        void AceptarPrestamo_ArgumentoInvalido_RetornaBadRequest();
        void AceptarPrestamo_ErrorServidor_RetornaError500();
    }
}
