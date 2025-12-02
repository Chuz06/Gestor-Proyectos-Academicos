using Gestor_Proyectos_Academicos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gestor_Proyectos_Academicos.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly GestorProyectosContext _context;

        public ReportesController(GestorProyectosContext context)
        {
            _context = context;
        }

        //  REPORTE GENERAL DE PROYECTOS (PROFESOR) 
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> Proyectos()
        {
            var datos = await _context.ReporteProyectos.ToListAsync();
            return View(datos);
        }

        //  REPORTE POR ESTUDIANTES (PROFESOR) 
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> Estudiantes()
        {
            var datos = await _context.ReporteEstudiantes.ToListAsync();
            return View(datos);
        }

        // REPORTE PERSONAL (ESTUDIANTE) 
        [Authorize(Roles = "Estudiante")]
        public async Task<IActionResult> MiResumen()
        {
            // usa el correo del usuario logueado
            var correo = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(correo))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var estudiante = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (estudiante == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var datos = await _context.ReporteEstudiantes
                .Where(r => r.IdEstudiante == estudiante.IdUsuario)
                .ToListAsync();

            return View(datos);
        }
    }
}
