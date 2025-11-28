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
    [AllowAnonymous] // permite entrar al Login sin estar autenticado
    public class CuentaController : Controller
    {
        private readonly GestorProyectosContext _context;

        public CuentaController(GestorProyectosContext context)
        {
            _context = context;
        }

        // ================== LOGIN GET ==================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ================== LOGIN POST ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            // 1) Buscar usuario por correo
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }

            // 2) Calcular hash SHA256 de la contraseña ingresada
            if (string.IsNullOrWhiteSpace(contrasena) ||
                string.IsNullOrWhiteSpace(usuario.Contrasena))
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }

            var hashIngresado = ComputeSha256Hash(contrasena);

            // 3) Comparar con lo que está en la BD (el script ya guardó SHA256 ahí)
            if (!string.Equals(hashIngresado, usuario.Contrasena, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }

            // 4) Claims del usuario
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

            // 5) Firmar cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            // 6) Redirigir al Home (o donde quieras)
            return RedirectToAction("Index", "Home");
        }

        // ================== LOGOUT ==================
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Cuenta");
        }

        // ================== ACCESS DENIED ==================
        public IActionResult Denegado()
        {
            return View();
        }

        // ================== HELPER SHA256 ==================
        private string ComputeSha256Hash(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    // IMPORTANTE: "X2" = HEX MAYÚSCULAS
                    builder.Append(b.ToString("X2"));
                }

                return builder.ToString();
            }
        }
    }
}
