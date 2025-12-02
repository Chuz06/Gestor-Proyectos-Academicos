using Gestor_Proyectos_Academicos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gestor_Proyectos_Academicos.Controllers
{
    [Authorize(Roles = "Profesor")]
    public class AsignacionesController : Controller
    {
        private readonly GestorProyectosContext _context;

        public AsignacionesController(GestorProyectosContext context)
        {
            _context = context;
        }

        // GET: Asignaciones/Index
        public async Task<IActionResult> Index()
        {
            var asignaciones = await _context.ProyectosEstudiantes
                .Include(pe => pe.Proyecto)
                .Include(pe => pe.Estudiante)
                .ToListAsync();

            return View(asignaciones);
        }

        // GET: Asignaciones/Create
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        private void CargarCombos()
        {
            ViewBag.Proyectos = new SelectList(_context.Proyectos, "IdProyecto", "Nombre");

            // solo estudiantes (IdRol = 3)
            ViewBag.Estudiantes = new SelectList(
                _context.Usuarios.Where(u => u.IdRol == 3),
                "IdUsuario",
                "Nombre"
            );
        }

        // POST: Asignaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProyectosEstudiantes modelo)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(modelo);
            }

            // evitar asignaciones duplicadas
            var existe = await _context.ProyectosEstudiantes
                .AnyAsync(pe => pe.IdProyecto == modelo.IdProyecto &&
                                pe.IdEstudiante == modelo.IdEstudiante);

            if (existe)
            {
                ModelState.AddModelError(string.Empty, "⚠️ El estudiante ya está asignado a ese proyecto.");
                CargarCombos();
                return View(modelo);
            }

            _context.ProyectosEstudiantes.Add(modelo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
