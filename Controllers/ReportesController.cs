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

        // REPORTERÍA DEL ADMINISTRADOR 

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ReportesAdmin()
        {
            // --- USUARIOS ---
            var totalUsuarios = await _context.Usuarios.CountAsync();
            var totalAdmins = await _context.Usuarios.CountAsync(u => u.IdRol == 1);
            var totalProfes = await _context.Usuarios.CountAsync(u => u.IdRol == 2);
            var totalEstudiantes = await _context.Usuarios.CountAsync(u => u.IdRol == 3);

            // --- PROYECTOS ---
            var totalProyectos = await _context.Proyectos.CountAsync();

            var activos = await _context.Proyectos
                .CountAsync(p => p.FechaFin >= DateTime.Today);

            var finalizados = await _context.Proyectos
                .CountAsync(p => p.FechaFin < DateTime.Today);

            // --- TAREAS ---
            var totalTareas = await _context.Tareas.CountAsync();
            var pendientes = await _context.Tareas.CountAsync(t => t.Estado == "Pendiente");
            var progreso = await _context.Tareas.CountAsync(t => t.Estado == "En Progreso");
            var completadas = await _context.Tareas.CountAsync(t => t.Estado == "Completada");

            ViewBag.TotalUsuarios = totalUsuarios;
            ViewBag.Admins = totalAdmins;
            ViewBag.Profesores = totalProfes;
            ViewBag.Estudiantes = totalEstudiantes;

            ViewBag.TotalProyectos = totalProyectos;
            ViewBag.Activos = activos;
            ViewBag.Finalizados = finalizados;

            ViewBag.TotalTareas = totalTareas;
            ViewBag.Pendientes = pendientes;
            ViewBag.Progreso = progreso;
            ViewBag.Completadas = completadas;

            return View();
        }

        // REPORTES PARA PROFESOR 

        // REPORTE GENERAL DE PROYECTOS (PROFESOR)
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> Proyectos()
        {
            var datos = await _context.ReporteProyectos.ToListAsync();
            return View(datos);
        }

        // REPORTE POR ESTUDIANTES (PROFESOR)
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
            var correo = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(correo))
                return RedirectToAction("Login", "Cuenta");

            var estudiante = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (estudiante == null)
                return RedirectToAction("Login", "Cuenta");

            var datos = await _context.ReporteEstudiantes
                .Where(r => r.IdEstudiante == estudiante.IdUsuario)
                .ToListAsync();

            return View(datos);
        }

    }
}
