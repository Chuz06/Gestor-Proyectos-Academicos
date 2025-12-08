using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestor_Proyectos_Academicos.Controllers
{
    public class DashboardsController : Controller
    {
        [Authorize(Roles = "Profesor")]
        public IActionResult ProfesorDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Estudiante")]
        public IActionResult EstudianteDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult AdministradorDashboard()
        {
            return View();
        }
    }
}
