using Microsoft.AspNetCore.Http;

namespace IMT_Reservas.Server.Shared.Common
{
    public class AceptarPrestamoComando
    {
        public int PrestamoId { get; set; }
        public string IdContrato { get; set; }
    }
}
