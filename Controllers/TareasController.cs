using Gestor_Proyectos_Academicos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace Gestor_Proyectos_Academicos.Controllers
{
    [Authorize(Roles = "Profesor,Estudiante,Administrador")]
    public class TareasController : Controller
    {
        private readonly GestorProyectosContext _context;

        public TareasController(GestorProyectosContext context)
        {
            _context = context;
        }

        // GET: Tareas
        public async Task<IActionResult> Index()
        {
            var tareas = await _context.Tareas.ToListAsync();
            return View(tareas);
        }

        // GET: Tareas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tarea = await _context.Tareas
                .FirstOrDefaultAsync(t => t.IdTarea == id);

            if (tarea == null) return NotFound();

            return View(tarea);
        }

        // GET: Tareas/Create
        public IActionResult Create()
        {
            // Todos los proyectos
            ViewBag.Proyectos = new SelectList(_context.Proyectos, "IdProyecto", "Nombre");

            // Todos los usuarios con rol Estudiante (asumo IdRol = 2 para estudiantes)
            ViewBag.Estudiantes = new SelectList(
                _context.Usuarios.Where(u => u.IdRol == 2),
                "IdUsuario",
                "Nombre"
            );

            return View();
        }


        // POST: Tareas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tarea);
        }

        // GET: Tareas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            // Proyectos con el proyecto actual seleccionado
            ViewBag.Proyectos = new SelectList(
                _context.Proyectos,
                "IdProyecto",
                "Nombre",
                tarea.IdProyecto
            );

            // Estudiantes con el estudiante actual seleccionado
            ViewBag.Estudiantes = new SelectList(
                _context.Usuarios.Where(u => u.IdRol == 2),
                "IdUsuario",
                "Nombre",
                tarea.IdAsignadoA
            );

            return View(tarea);
        }


        // POST: Tareas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tarea tarea)
        {
            if (id != tarea.IdTarea) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TareaExists(tarea.IdTarea))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tarea);
        }

        // GET: Tareas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tarea = await _context.Tareas
                .FirstOrDefaultAsync(t => t.IdTarea == id);

            if (tarea == null) return NotFound();

            return View(tarea);
        }

        // POST: Tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea != null)
            {
                _context.Tareas.Remove(tarea);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TareaExists(int id)
        {
            return _context.Tareas.Any(e => e.IdTarea == id);
        }
    }
}
