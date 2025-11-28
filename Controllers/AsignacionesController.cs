using System.Linq;
using System.Threading.Tasks;
using Gestor_Proyectos_Academicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;



namespace Gestor_Proyectos_Academicos.Controllers
{
    [Authorize(Roles = "Profesor,Administrador")]
    public class AsignacionesController : Controller
    {
        private readonly GestorProyectosContext _context;

        public AsignacionesController(GestorProyectosContext context)
        {
            _context = context;
        }

        // GET: Asignaciones
        public async Task<IActionResult> Index()
        {
            var asignaciones = await _context.ProyectosEstudiantes
                .Include(pe => pe.Proyecto)
                .Include(pe => pe.Estudiante)
                .ToListAsync();

            return View(asignaciones);
        }

        // GET: Asignaciones/Details?idProyecto=1&idEstudiante=2
        public async Task<IActionResult> Details(int? idProyecto, int? idEstudiante)
        {
            if (idProyecto == null || idEstudiante == null)
                return NotFound();

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

        // GET: Asignaciones/Create
        public IActionResult Create()
        {
            ViewData["IdProyecto"] = new SelectList(_context.Proyectos, "IdProyecto", "Nombre");
            ViewData["IdEstudiante"] = new SelectList(_context.Usuarios, "IdUsuario", "Nombre"); // luego podrías filtrar por rol "Estudiante"
            return View();
        }

        // POST: Asignaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProyectosEstudiantes asignacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asignacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdProyecto"] = new SelectList(_context.Proyectos, "IdProyecto", "Nombre", asignacion.IdProyecto);
            ViewData["IdEstudiante"] = new SelectList(_context.Usuarios, "IdUsuario", "Nombre", asignacion.IdEstudiante);
            return View(asignacion);
        }

        // GET: Asignaciones/Edit?idProyecto=1&idEstudiante=2
        public async Task<IActionResult> Edit(int? idProyecto, int? idEstudiante)
        {
            if (idProyecto == null || idEstudiante == null)
                return NotFound();

            var asignacion = await _context.ProyectosEstudiantes
                .FirstOrDefaultAsync(pe =>
                    pe.IdProyecto == idProyecto &&
                    pe.IdEstudiante == idEstudiante);

            if (asignacion == null)
                return NotFound();

            ViewData["IdProyecto"] = new SelectList(_context.Proyectos, "IdProyecto", "Nombre", asignacion.IdProyecto);
            ViewData["IdEstudiante"] = new SelectList(_context.Usuarios, "IdUsuario", "Nombre", asignacion.IdEstudiante);

            return View(asignacion);
        }

        // POST: Asignaciones/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idProyectoOriginal, int idEstudianteOriginal, ProyectosEstudiantes asignacion)
        {
            // Para simplificar: si cambian el proyecto o estudiante, eliminamos la vieja y creamos la nueva
            if (ModelState.IsValid)
            {
                var original = await _context.ProyectosEstudiantes
                    .FirstOrDefaultAsync(pe =>
                        pe.IdProyecto == idProyectoOriginal &&
                        pe.IdEstudiante == idEstudianteOriginal);

                if (original == null)
                    return NotFound();

                _context.ProyectosEstudiantes.Remove(original);
                _context.ProyectosEstudiantes.Add(asignacion);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdProyecto"] = new SelectList(_context.Proyectos, "IdProyecto", "Nombre", asignacion.IdProyecto);
            ViewData["IdEstudiante"] = new SelectList(_context.Usuarios, "IdUsuario", "Nombre", asignacion.IdEstudiante);
            return View(asignacion);
        }

        // GET: Asignaciones/Delete
        public async Task<IActionResult> Delete(int? idProyecto, int? idEstudiante)
        {
            if (idProyecto == null || idEstudiante == null)
                return NotFound();

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

        // POST: Asignaciones/Delete
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
