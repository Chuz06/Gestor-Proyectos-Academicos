using System.ComponentModel.DataAnnotations;

namespace Gestor_Proyectos_Academicos.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }
    }
}
