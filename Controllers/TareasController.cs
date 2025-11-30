using Gestor_Proyectos_Academicos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        //  LISTA 
        public async Task<IActionResult> Index()
        {
            var tareas = await _context.Tareas
                .Include(t => t.Proyecto)
                .Include(t => t.AsignadoA)
                .ToListAsync();

            return View(tareas);
        }

        //  DETALLES
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tarea = await _context.Tareas
                .Include(t => t.Proyecto)
                .Include(t => t.AsignadoA)
                .FirstOrDefaultAsync(t => t.IdTarea == id);

            if (tarea == null) return NotFound();

            return View(tarea);
        }

        //LISTA DE ESTADOS 
        private IEnumerable<SelectListItem> ObtenerEstados()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem { Value = "Pendiente",   Text = "Pendiente" },
                    new SelectListItem { Value = "En Progreso", Text = "En Progreso" },
                    new SelectListItem { Value = "Completada",  Text = "Completada" }
                };
        }

        private void CargarCombos(Tarea? tarea = null)
        {
            ViewBag.Proyectos = new SelectList(
                _context.Proyectos,
                "IdProyecto",
                "Nombre",
                tarea?.IdProyecto
            );

            ViewBag.Estudiantes = new SelectList(
                _context.Usuarios.Where(u => u.IdRol == 3),
                "IdUsuario",
                "Nombre",
                tarea?.IdAsignadoA
            );

            ViewBag.Estados = new SelectList(
                ObtenerEstados(),
                "Value",
                "Text",
                tarea?.Estado ?? "Pendiente"
            );
        }

        //  CREAR GET 
        public IActionResult Create()
        {
            CargarCombos();
            return View(new Tarea
            {
                Estado = "Pendiente"
            });
        }

        //   CREAR POST     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarea tarea)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(tarea);
                return View(tarea);
            }

            try
            {
                _context.Add(tarea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                
                ModelState.AddModelError(string.Empty,
                    $"Error al guardar la tarea: {ex.InnerException?.Message ?? ex.Message}");
                CargarCombos(tarea);
                return View(tarea);
            }
        }

        //  EDITAR GET 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tarea = await _context.Tareas
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdTarea == id);

            if (tarea == null) return NotFound();

            ViewBag.Proyectos = new SelectList(_context.Proyectos, "IdProyecto", "Nombre", tarea.IdProyecto);

            ViewBag.Estudiantes = new SelectList(
                _context.Usuarios.Where(u => u.IdRol == 3),
                "IdUsuario",
                "Nombre",
                tarea.IdAsignadoA
            );

            ViewBag.Estados = new SelectList(ObtenerEstados(), "Value", "Text", tarea.Estado);

            return View(tarea);
        }

        //  EDITAR POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTarea,Titulo,Descripcion,FechaLimite,Estado,IdProyecto,IdAsignadoA")] Tarea modelo)
        {
            if (id != modelo.IdTarea)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                
                ViewBag.Proyectos = new SelectList(_context.Proyectos, "IdProyecto", "Nombre", modelo.IdProyecto);
                ViewBag.Estudiantes = new SelectList(
                    _context.Usuarios.Where(u => u.IdRol == 3),
                    "IdUsuario",
                    "Nombre",
                    modelo.IdAsignadoA
                );
                ViewBag.Estados = new SelectList(ObtenerEstados(), "Value", "Text", modelo.Estado);

                return View(modelo);
            }

            try
            {
                // Cargamos la tarea real de la BD
                var tareaDb = await _context.Tareas.FirstOrDefaultAsync(t => t.IdTarea == id);

                if (tareaDb == null)
                    return NotFound();

                //  
                tareaDb.Titulo = modelo.Titulo;
                tareaDb.Descripcion = modelo.Descripcion;
                tareaDb.FechaLimite = modelo.FechaLimite;
                tareaDb.Estado = modelo.Estado;
                tareaDb.IdProyecto = modelo.IdProyecto;
                tareaDb.IdAsignadoA = modelo.IdAsignadoA;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                
                ModelState.AddModelError(string.Empty, $"Error al guardar la tarea: {ex.Message}");

                ViewBag.Proyectos = new SelectList(_context.Proyectos, "IdProyecto", "Nombre", modelo.IdProyecto);
                ViewBag.Estudiantes = new SelectList(
                    _context.Usuarios.Where(u => u.IdRol == 3),
                    "IdUsuario",
                    "Nombre",
                    modelo.IdAsignadoA
                );
                ViewBag.Estados = new SelectList(ObtenerEstados(), "Value", "Text", modelo.Estado);

                return View(modelo);
            }
        }


        //  ELIMINAR POST 
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
    }
}
