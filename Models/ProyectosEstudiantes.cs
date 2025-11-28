using System.ComponentModel.DataAnnotations.Schema;


using System.Collections.Generic;

namespace Gestor_Proyectos_Academicos.Models
{
    public class ProyectosEstudiantes
    {
        // Clave compuesta
        public int IdProyecto { get; set; }
        public int IdEstudiante { get; set; }

        // 🔹 Navegaciones (LAS QUE FALTAN)
        public Proyecto Proyecto { get; set; }
        public Usuario Estudiante { get; set; }
    }
}

