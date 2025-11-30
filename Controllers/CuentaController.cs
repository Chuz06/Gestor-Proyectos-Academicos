using Gestor_Proyectos_Academicos.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Gestor_Proyectos_Academicos.Controllers
{
    [AllowAnonymous]  
    public class CuentaController : Controller
    {
        private readonly GestorProyectosContext _context;

        public CuentaController(GestorProyectosContext context)
        {
            _context = context;
        }

        // LOGIN GET 
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            // usuario por correo
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }

            //  Calcula hash SHA256 de la contraseña 
            if (string.IsNullOrWhiteSpace(contrasena) ||
                string.IsNullOrWhiteSpace(usuario.Contrasena))
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }

            var hashIngresado = ComputeSha256Hash(contrasena);

            //  Comparar con lo que está en la BD 
            if (!string.Equals(hashIngresado, usuario.Contrasena, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }

            //  Claims del usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol?.NombreRol ?? "Usuario")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(claimsIdentity);

            //  Firmar cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            //  Redirigir al Home 
            return RedirectToAction("Index", "Home");
        }

        //  LOGOUT 
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Cuenta");
        }

        // ACCESS DENIED 
        public IActionResult Denegado()
        {
            return View();
        }

        // HELPER SHA256 
        private string ComputeSha256Hash(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    //"X2" = HEX MAYÚSCULAS
                    builder.Append(b.ToString("X2"));
                }

                return builder.ToString();
            }
        }
    }
}
