using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestor_Proyectos_Academicos.Models
{
    public class Proyecto
    {
        [Key]
        public int IdProyecto { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        [ForeignKey(nameof(Profesor))]
        public int IdProfesor { get; set; }

        [ValidateNever]
        public Usuario? Profesor { get; set; }
    }
}
