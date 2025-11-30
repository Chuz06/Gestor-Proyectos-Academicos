using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Proyectos_Academicos.Models;
using Microsoft.AspNetCore.Authorization;


namespace Gestor_Proyectos_Academicos.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RolesController : Controller
    {
        private readonly GestorProyectosContext _context;

        public RolesController(GestorProyectosContext context)
        {
            _context = context;
        }

        //  Roles
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.ToListAsync();
            return View(roles);
        }

        //  Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var rol = await _context.Roles
                .FirstOrDefaultAsync(m => m.IdRol == id);

            if (rol == null) return NotFound();

            return View(rol);
        }

        //  Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        //  Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreRol")] Rol rol)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rol);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rol);
        }

        //  Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return NotFound();

            return View(rol);
        }

        // : Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRol,NombreRol")] Rol rol)
        {
            if (id != rol.IdRol) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rol);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolExists(rol.IdRol))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rol);
        }

        // Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rol = await _context.Roles
                .FirstOrDefaultAsync(m => m.IdRol == id);

            if (rol == null) return NotFound();

            return View(rol);
        }

        //  Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol != null)
            {
                _context.Roles.Remove(rol);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RolExists(int id)
        {
            return _context.Roles.Any(e => e.IdRol == id);
        }
    }
}
