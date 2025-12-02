using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Gestor_Proyectos_Academicos.Models
{
    public class ProyectosEstudiantes
    {
        [Required]
        public int IdProyecto { get; set; }

        [Required]
        public int IdEstudiante { get; set; }

        // Propiedades de navegación: NO se validan en el POST
        [ValidateNever]
        public Proyecto? Proyecto { get; set; }

        [ValidateNever]
        public Usuario? Estudiante { get; set; }
    }
}
