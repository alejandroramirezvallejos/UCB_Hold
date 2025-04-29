using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace IMT_Reservas.Server.Controllers
{


    [Route("api/producto")]
    [ApiController]

    public class ProductoController : ControllerBase
    {

        [HttpGet("{id}")]
        public IActionResult Index(string id)
        {
            if (!int.TryParse(id, out int ids))
            {
                return BadRequest("El ID proporcionado no es numérico.");
            }


            var parametro = new Dictionary<string, object>();

            var conexion = new NpgsqlConnection("Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=273153");

            conexion.Open();

            var comando = new NpgsqlCommand("SELECT * FROM public.grupos_equipos where id_grupo_equipo=@id", conexion);
            comando.Parameters.AddWithValue("id", ids);
            var resultado = comando.ExecuteReader();

            var mandar = new List<object>();

            while (resultado.Read())
            {
                var fila = new
                {
                    id = resultado["id_grupo_equipo"].ToString(),
                    nombre = resultado["nombre"].ToString(),
                    link = resultado["link"].ToString(),
                    marca = resultado["marca"].ToString(),
                    modelo = resultado["modelo"].ToString(),
                    descripcion = resultado["descripcion"].ToString(),
                    url_data_sheet = resultado["url_data_sheet"].ToString(),
                };

                mandar.Add(fila);
            }

            conexion.Close();
            return Ok(mandar);
        }
    }
}
