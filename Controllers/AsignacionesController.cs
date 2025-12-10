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

        public async Task<IActionResult> Index()
        {
            var asignaciones = await _context.ProyectosEstudiantes
                .Include(pe => pe.Proyecto)
                .Include(pe => pe.Estudiante)
                .ToListAsync();

            return View(asignaciones);
        }

        private void CargarCombos(int? idProyecto = null, int? idEstudiante = null)
        {
            ViewBag.Proyectos = new SelectList(
                _context.Proyectos,
                "IdProyecto",
                "Nombre",
                idProyecto
            );

            ViewBag.Estudiantes = new SelectList(
                _context.Usuarios.Where(u => u.IdRol == 3),
                "IdUsuario",
                "Nombre",
                idEstudiante
            );
        }

        // crear get
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        // crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProyectosEstudiantes modelo)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(modelo);
            }

            var existe = await _context.ProyectosEstudiantes
                .AnyAsync(pe => pe.IdProyecto == modelo.IdProyecto &&
                                pe.IdEstudiante == modelo.IdEstudiante);

            if (existe)
            {
                ModelState.AddModelError("", "⚠️ El estudiante ya está asignado a este proyecto.");
                CargarCombos();
                return View(modelo);
            }

            _context.Add(modelo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // detalles
        public async Task<IActionResult> Details(int idProyecto, int idEstudiante)
        {
            var asignacion = await _context.ProyectosEstudiantes
                .Include(pe => pe.Proyecto)
                .Include(pe => pe.Estudiante)
                .FirstOrDefaultAsync(pe =>
                    pe.IdProyecto == idProyecto &&
                    pe.IdEstudiante == idEstudiante);

            if (asignacion == null)
                return NotFound();

            return View(asignacion);
        }

        // edit get
        public async Task<IActionResult> Edit(int idProyecto, int idEstudiante)
        {
            var asignacion = await _context.ProyectosEstudiantes
                .FirstOrDefaultAsync(pe =>
                    pe.IdProyecto == idProyecto &&
                    pe.IdEstudiante == idEstudiante);

            if (asignacion == null)
                return NotFound();

            CargarCombos(asignacion.IdProyecto, asignacion.IdEstudiante);
            return View(asignacion);
        }

        // edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int idProyectoOriginal,
            int idEstudianteOriginal,
            ProyectosEstudiantes modelo)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(modelo.IdProyecto, modelo.IdEstudiante);
                return View(modelo);
            }

            // 1. Buscar la asignación original
            var asignacionOriginal = await _context.ProyectosEstudiantes
                .FirstOrDefaultAsync(pe =>
                    pe.IdProyecto == idProyectoOriginal &&
                    pe.IdEstudiante == idEstudianteOriginal);

            if (asignacionOriginal == null)
                return NotFound();

            // 2. Eliminar la asignación vieja
            _context.ProyectosEstudiantes.Remove(asignacionOriginal);
            await _context.SaveChangesAsync();

            // 3. Crear la nueva asignación (con los valores modificados)
            var nueva = new ProyectosEstudiantes
            {
                IdProyecto = modelo.IdProyecto,
                IdEstudiante = modelo.IdEstudiante
            };

            _context.ProyectosEstudiantes.Add(nueva);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // delete get
        public async Task<IActionResult> Delete(int idProyecto, int idEstudiante)
        {
            var asignacion = await _context.ProyectosEstudiantes
                .Include(pe => pe.Proyecto)
                .Include(pe => pe.Estudiante)
                .FirstOrDefaultAsync(pe =>
                    pe.IdProyecto == idProyecto &&
                    pe.IdEstudiante == idEstudiante);

            if (asignacion == null)
                return NotFound();

            return View(asignacion);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int idProyecto, int idEstudiante)
        {
            var asignacion = await _context.ProyectosEstudiantes
                .FirstOrDefaultAsync(pe =>
                    pe.IdProyecto == idProyecto &&
                    pe.IdEstudiante == idEstudiante);

            if (asignacion != null)
            {
                _context.ProyectosEstudiantes.Remove(asignacion);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
