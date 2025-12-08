using Gestor_Proyectos_Academicos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gestor_Proyectos_Academicos.Controllers
{
    [Authorize] // solo requiere login
    public class ProyectosController : Controller
    {
        private readonly GestorProyectosContext _context;

        public ProyectosController(GestorProyectosContext context)
        {
            _context = context;
        }

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

            // ADMINISTRADOR → ve todos
            if (rol == "Administrador")
            {
                proyectos = await _context.Proyectos.ToListAsync();
            }

            // PROFESOR → ve los que él creó
            else if (rol == "Profesor")
            {
                proyectos = await _context.Proyectos
                    .Where(p => p.IdProfesor == usuario.IdUsuario)
                    .ToListAsync();
            }

            // ESTUDIANTE → ve solo los proyectos donde está asignado
            else
            {
                proyectos = await _context.ProyectosEstudiantes.Where(pe => pe.IdEstudiante == usuario.IdUsuario).Select(pe => pe.Proyecto).Distinct().ToListAsync();
            }

            return View(proyectos);
        }

        // detalles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var proyecto = await _context.Proyectos
                .FirstOrDefaultAsync(p => p.IdProyecto == id);

            if (proyecto == null) return NotFound();

            return View(proyecto);
        }

        // create, solo admin y profe
        [Authorize(Roles = "Profesor,Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Profesor,Administrador")]
        public async Task<IActionResult> Create(Proyecto proyecto)
        {
            var correo = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            proyecto.IdProfesor = usuario.IdUsuario;

            if (ModelState.IsValid)
            {
                _context.Add(proyecto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(proyecto);
        }

        // edit
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

            return View(proyecto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Profesor,Administrador")]
        public async Task<IActionResult> Edit(int id, Proyecto proyecto)
        {
            if (id != proyecto.IdProyecto) return NotFound();

            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            var correo = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            if (rol == "Profesor" && proyecto.IdProfesor != usuario.IdUsuario)
                return Forbid();

            if (ModelState.IsValid)
            {
                _context.Update(proyecto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(proyecto);
        }
        
        // delete
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

            _context.Proyectos.Remove(proyecto!);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
