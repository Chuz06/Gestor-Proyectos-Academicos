using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestor_Proyectos_Academicos.Models
{
    [Table("Tareas")]
    public class Tarea
    {
        [Key]
        public int IdTarea { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public DateTime? FechaLimite { get; set; }

        public string Estado { get; set; }

        public int IdProyecto { get; set; }

        public int IdAsignadoA { get; set; }
    }
}
