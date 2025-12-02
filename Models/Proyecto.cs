using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestor_Proyectos_Academicos.Models
{
    [Table("Proyectos")]
    public class Proyecto
    {
        [Key]
        public int IdProyecto { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        [Required]
        public int IdProfesor { get; set; }
    }
}
