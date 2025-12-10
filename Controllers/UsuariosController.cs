using Gestor_Proyectos_Academicos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gestor_Proyectos_Academicos.Controllers
{
    [Authorize(Roles = "Administrador, Profesor")]
    public class UsuariosController : Controller
    {
        private readonly GestorProyectosContext _context;

        public UsuariosController(GestorProyectosContext context)
        {
            _context = context;
        }

        // LISTA
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .ToListAsync();

            return View(usuarios);
        }

        // DETALLES
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // GET CREATE
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_context.Roles, "IdRol", "NombreRol");
            return View();
        }

        // POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Encripta la contraseña antes de guardar
                usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }

        // GET EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null) return NotFound();

            ViewBag.Roles = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);

            return View(usuario);
        }

        // POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Si NO cambia la contraseña, conservar la original
                    if (!string.IsNullOrWhiteSpace(usuario.Contrasena))
                    {
                        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
                    }
                    else
                    {
                        var usuarioOriginal = await _context.Usuarios
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.IdUsuario == id);

                        if (usuarioOriginal != null)
                            usuario.Contrasena = usuarioOriginal.Contrasena;
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.IdUsuario == usuario.IdUsuario))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }

        // GET DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Usuario eliminado correctamente";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
