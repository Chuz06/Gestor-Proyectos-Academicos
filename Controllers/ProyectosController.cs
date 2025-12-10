using Gestor_Proyectos_Academicos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gestor_Proyectos_Academicos.Controllers
{
    [Authorize]
    public class ProyectosController : Controller
    {
        private readonly GestorProyectosContext _context;

        public ProyectosController(GestorProyectosContext context)
        {
            _context = context;
        }

        // INDEX 
        [Authorize(Roles = "Profesor,Estudiante,Administrador")]
        public async Task<IActionResult> Index()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            var correo = User.FindFirst(ClaimTypes.Email)?.Value;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
                return Unauthorized();

            List<Proyecto> proyectos;

            if (rol == "Administrador")
            {
                proyectos = await _context.Proyectos.ToListAsync();
            }
            else if (rol == "Profesor")
            {
                proyectos = await _context.Proyectos
                    .Where(p => p.IdProfesor == usuario.IdUsuario)
                    .ToListAsync();
            }
            else
            {
                proyectos = await _context.ProyectosEstudiantes
                    .Where(pe => pe.IdEstudiante == usuario.IdUsuario)
                    .Select(pe => pe.Proyecto)
                    .Distinct()
                    .ToListAsync();
            }

            return View(proyectos);
        }

        // DETAILS 
        public async Task<IActionResult> Details(int id)
        {
            var proyecto = await _context.Proyectos
                .FirstOrDefaultAsync(p => p.IdProyecto == id);

            if (proyecto == null)
                return NotFound();

            ViewBag.NombreProfesor = await _context.Usuarios
                .Where(u => u.IdUsuario == proyecto.IdProfesor)
                .Select(u => u.Nombre)
                .FirstOrDefaultAsync();

            return View(proyecto);
        }

        // CREATE GET 
        [Authorize(Roles = "Profesor,Administrador")]
        public IActionResult Create()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (rol == "Administrador")
            {
                ViewBag.Profesores = new SelectList(
                    _context.Usuarios.Where(u => u.IdRol == 2), 
                    "IdUsuario",
                    "Nombre"
                );
            }
            else
            {
                ViewBag.Profesores = null;
            }

            return View();
        }


        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Profesor,Administrador")]
        public async Task<IActionResult> Create(Proyecto proyecto)
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            var correo = User.FindFirst(ClaimTypes.Email)?.Value;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
                return Unauthorized();

            if (rol == "Profesor")
            {
                proyecto.IdProfesor = usuario.IdUsuario;

                ModelState.Remove("IdProfesor");
                ModelState.Remove("Profesor");
            }

            if (rol == "Administrador")
            {
                if (proyecto.IdProfesor <= 0)
                    ModelState.AddModelError("IdProfesor", "Debe seleccionar un profesor.");
            }

            if (!ModelState.IsValid)
            {
                if (rol == "Administrador")
                {
                    ViewBag.Profesores = new SelectList(
                        _context.Usuarios.Where(u => u.IdRol == 2),
                        "IdUsuario",
                        "Nombre",
                        proyecto.IdProfesor   
                    );
                }

                return View(proyecto);
            }

            _context.Add(proyecto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // EDIT GET
        [Authorize(Roles = "Profesor,Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null) return NotFound();

            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            var correo = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            if (rol == "Profesor" && proyecto.IdProfesor != usuario.IdUsuario)
                return Forbid();

            if (rol == "Administrador")
            {
                ViewBag.Profesores = new SelectList(
                    _context.Usuarios.Where(u => u.IdRol == 2), // 2 = Profesor
                    "IdUsuario",
                    "Nombre",
                    proyecto.IdProfesor
                );
            }

            return View(proyecto);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Profesor,Administrador")]
        public async Task<IActionResult> Edit(int id, Proyecto proyecto)
        {
            if (id != proyecto.IdProyecto) return NotFound();

            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            var correo = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            if (rol == "Profesor")
            {
                proyecto.IdProfesor = usuario.IdUsuario;
            }

            if (ModelState.IsValid)
            {
                _context.Update(proyecto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (rol == "Administrador")
            {
                ViewBag.Profesores = new SelectList(
                    _context.Usuarios.Where(u => u.IdRol == 2),
                    "IdUsuario",
                    "Nombre",
                    proyecto.IdProfesor
                );
            }

            return View(proyecto);
        }

        // DELETE 
        [Authorize(Roles = "Profesor,Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null) return NotFound();

            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            var correo = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            if (rol == "Profesor" && proyecto.IdProfesor != usuario.IdUsuario)
                return Forbid();

            return View(proyecto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Profesor,Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto != null)
            {
                _context.Proyectos.Remove(proyecto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
