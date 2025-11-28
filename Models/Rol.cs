using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gestor_Proyectos_Academicos.Models
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }
        public string NombreRol { get; set; }

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
