    using Gestor_Proyectos_Academicos.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Claims;

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

            private bool EsEstudiante() => User.IsInRole("Estudiante");

            private int ObtenerUsuarioActual()
            {
                var correo = User.FindFirst(ClaimTypes.Email)?.Value;

                if (correo == null)
                    throw new Exception("No se encontró el correo del usuario autenticado.");

                var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

                if (usuario == null)
                    throw new Exception("No se encontró el usuario en la base de datos.");

                return usuario.IdUsuario;
            }

            private IEnumerable<SelectListItem> ObtenerEstados()
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                    new SelectListItem { Value = "En Progreso", Text = "En Progreso" },
                    new SelectListItem { Value = "Completada", Text = "Completada" }
                };
            }

            private void CargarCombos(Tarea tarea, bool permitirSeleccionAsignado)
            {
                ViewBag.Proyectos = new SelectList(
                    _context.Proyectos,
                    "IdProyecto",
                    "Nombre",
                    tarea.IdProyecto);

                if (permitirSeleccionAsignado)
                {
                    ViewBag.Estudiantes = new SelectList(
                        _context.Usuarios.Where(u => u.IdRol == 3),
                        "IdUsuario",
                        "Nombre",
                        tarea.IdAsignadoA
                    );
                }

                ViewBag.Estados = new SelectList(
                    ObtenerEstados(),
                    "Value",
                    "Text",
                    tarea.Estado
                );
            }

            // INDEX
            public async Task<IActionResult> Index()
            {
                var tareas = await _context.Tareas
                    .Include(t => t.Proyecto)
                    .Include(t => t.AsignadoA)
                    .ToListAsync();

                return View(tareas);
            }

            // DETALLES
            public async Task<IActionResult> Details(int id)
            {
                var tarea = await _context.Tareas
                    .Include(t => t.Proyecto)
                    .Include(t => t.AsignadoA)
                    .FirstOrDefaultAsync(t => t.IdTarea == id);

                if (tarea == null) return NotFound();

                return View(tarea);
            }

            // CREATE GET
            public IActionResult Create()
            {
                var tarea = new Tarea { Estado = "Pendiente" };
                CargarCombos(tarea, !EsEstudiante());
                return View(tarea);
            }

            // CREATE POST
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Tarea tarea)
            {
                if (EsEstudiante())
                    tarea.IdAsignadoA = ObtenerUsuarioActual();

                var proyecto = await _context.Proyectos
                    .FirstOrDefaultAsync(p => p.IdProyecto == tarea.IdProyecto);

                if (proyecto == null)
                {
                    ModelState.AddModelError("IdProyecto", "Debe seleccionar un proyecto válido.");
                }
                else
                {
                    tarea.FechaLimite = proyecto.FechaFin;
                }

                if (!ModelState.IsValid)
                {
                    CargarCombos(tarea, !EsEstudiante());
                    return View(tarea);
                }

                _context.Add(tarea);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // EDIT GET
            public async Task<IActionResult> Edit(int id)
            {
                var tarea = await _context.Tareas.FindAsync(id);
                if (tarea == null) return NotFound();

                bool permitirAsignado = User.IsInRole("Profesor") || User.IsInRole("Administrador");

                CargarCombos(tarea, permitirAsignado);
                return View(tarea);
            }

            // EDIT POST
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, Tarea modelo)
            {
                if (id != modelo.IdTarea) return NotFound();

                var tareaDb = await _context.Tareas.FindAsync(id);
                if (tareaDb == null) return NotFound();

                if (EsEstudiante())
                {
                    tareaDb.Titulo = modelo.Titulo;
                    tareaDb.Descripcion = modelo.Descripcion;
                    tareaDb.Estado = modelo.Estado;
                }
                else
                {
                    tareaDb.Titulo = modelo.Titulo;
                    tareaDb.Descripcion = modelo.Descripcion;
                    tareaDb.Estado = modelo.Estado;
                    tareaDb.IdProyecto = modelo.IdProyecto;
                    tareaDb.IdAsignadoA = modelo.IdAsignadoA;

                    var proyecto = await _context.Proyectos
                        .FirstOrDefaultAsync(p => p.IdProyecto == modelo.IdProyecto);

                    if (proyecto != null)
                        tareaDb.FechaLimite = proyecto.FechaFin;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // DELETE GET
            public async Task<IActionResult> Delete(int id)
            {
                var tarea = await _context.Tareas
                    .Include(t => t.Proyecto)
                    .Include(t => t.AsignadoA)
                    .FirstOrDefaultAsync(t => t.IdTarea == id);

                if (tarea == null) return NotFound();

                return View(tarea);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int idTarea)
            {
                var tarea = await _context.Tareas.FindAsync(idTarea);

                if (tarea != null)
                {
                    _context.Tareas.Remove(tarea);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

        }
    }
