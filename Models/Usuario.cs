using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestor_Proyectos_Academicos.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, StringLength(100)]
        public string Correo { get; set; }

        [Required]
        public string Contrasena { get; set; }

        // ⚠️ ESTA es la FK REAL en tu BD
        [ForeignKey("Rol")]
        public int IdRol { get; set; }

        // Navegación
        public Rol Rol { get; set; }
    }
}
