using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Gestor_Proyectos_Academicos.Models
{
    public class Tarea
    {
        [Key]
        public int IdTarea { get; set; }

        [Required]
        public string Titulo { get; set; } = null!;

        public string? Descripcion { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaLimite { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = null!;

        [ForeignKey(nameof(Proyecto))]
        public int IdProyecto { get; set; }

        [ForeignKey(nameof(AsignadoA))]
        public int IdAsignadoA { get; set; }

        // --- NAVIGATIONS: no las valides en el modelo ---
        [ValidateNever]
        public Proyecto? Proyecto { get; set; }

        [ValidateNever]
        public Usuario? AsignadoA { get; set; }
    }
}