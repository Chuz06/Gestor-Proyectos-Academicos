using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net.NetworkInformation;

namespace Gestor_Proyectos_Academicos.Controllers
{
    public class PruebaController : Controller
    {
        public IActionResult Conexion()
        {
            string cadena = "Server=localhost; Database=GestorProyectos; TrustServerCertificate=true; Integrated Security=true; Encrypt=False; ";

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    conexion.Open();
                }
                return Content("Conexión exitosa a la base de datos");
            }
            catch (Exception ex)
            {
                return Content("Error en la conexión: " + ex.Message);
            }
        }
    }
}