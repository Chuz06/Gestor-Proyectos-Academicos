using System;

namespace Gestor_Proyectos_Academicos.Models
{
    // Mapea a vw_ReporteProyectos
    public class ReporteProyectoView
    {
        public int IdProyecto { get; set; }
        public string Proyecto { get; set; } = null!;
        public string? Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Profesor { get; set; } = null!;
        public int TotalTareas { get; set; }
        public int TareasCompletadas { get; set; }

        public double PorcentajeAvance =>
            TotalTareas == 0 ? 0 : (TareasCompletadas * 100.0 / TotalTareas);
    }

    // Mapea a vw_ReporteEstudiantes
    public class ReporteEstudianteView
    {
        public int IdEstudiante { get; set; }
        public string Estudiante { get; set; } = null!;
        public string Proyecto { get; set; } = null!;
        public int TotalTareas { get; set; }
        public int TareasCompletadas { get; set; }

        public double PorcentajeAvance =>
            TotalTareas == 0 ? 0 : (TareasCompletadas * 100.0 / TotalTareas);
    }
}
