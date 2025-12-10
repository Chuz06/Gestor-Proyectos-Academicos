using Microsoft.EntityFrameworkCore;

namespace Gestor_Proyectos_Academicos.Models
{
    public class GestorProyectosContext : DbContext
    {
        public GestorProyectosContext(DbContextOptions<GestorProyectosContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<ProyectosEstudiantes> ProyectosEstudiantes { get; set; }

        public DbSet<ReporteProyectoView> ReporteProyectos { get; set; }
        public DbSet<ReporteEstudianteView> ReporteEstudiantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USUARIO -> ROL
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.IdRol)
                .OnDelete(DeleteBehavior.Restrict);


            // PROYECTO -> PROFESOR
            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.Profesor)
                .WithMany()
                .HasForeignKey(p => p.IdProfesor)
                .OnDelete(DeleteBehavior.Cascade);


            // TAREAS -> PROYECTO
            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.Proyecto)
                .WithMany()
                .HasForeignKey(t => t.IdProyecto)
                .OnDelete(DeleteBehavior.Cascade);


            // TAREAS -> ASIGNADO A (Usuario)
            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.AsignadoA)
                .WithMany()
                .HasForeignKey(t => t.IdAsignadoA)
                .OnDelete(DeleteBehavior.Cascade);


            //  PROYECTO - ESTUDIANTE
            modelBuilder.Entity<ProyectosEstudiantes>(entity =>
            {
                entity.HasKey(pe => new { pe.IdProyecto, pe.IdEstudiante });

                entity.HasOne(pe => pe.Proyecto)
                      .WithMany()
                      .HasForeignKey(pe => pe.IdProyecto)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pe => pe.Estudiante)
                      .WithMany()
                      .HasForeignKey(pe => pe.IdEstudiante)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // VISTAS
            modelBuilder.Entity<ReporteProyectoView>()
                .HasNoKey()
                .ToView("vw_ReporteProyectos");

            modelBuilder.Entity<ReporteEstudianteView>()
                .HasNoKey()
                .ToView("vw_ReporteEstudiantes");
        }
    }
}
