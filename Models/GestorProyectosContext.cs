using Microsoft.EntityFrameworkCore;

namespace Gestor_Proyectos_Academicos.Models
{
    public class GestorProyectosContext : DbContext
    {
        public GestorProyectosContext(DbContextOptions<GestorProyectosContext> options)
            : base(options)
        {
        }

        // ========= Tablas =========
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<ProyectosEstudiantes> ProyectosEstudiantes { get; set; }

        // ========= Vistas de reporte =========
        public DbSet<ReporteProyectoView> ReporteProyectos { get; set; }
        public DbSet<ReporteEstudianteView> ReporteEstudiantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- USUARIOS / ROLES ----
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.IdUsuario);

                entity.Property(u => u.IdRol)
                      .HasColumnName("IdRol");

                entity.HasOne(u => u.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(u => u.IdRol);
            });

            // ---- PROYECTOS-ESTUDIANTES (tabla intermedia) ----
            modelBuilder.Entity<ProyectosEstudiantes>(entity =>
            {
                // PK compuesta
                entity.HasKey(pe => new { pe.IdProyecto, pe.IdEstudiante });

                entity.HasOne(pe => pe.Proyecto)
                      .WithMany()              // opcionalmente: .WithMany(p => p.ProyectosEstudiantes)
                      .HasForeignKey(pe => pe.IdProyecto);

                entity.HasOne(pe => pe.Estudiante)
                      .WithMany()              // opcionalmente: .WithMany(u => u.ProyectosEstudiantes)
                      .HasForeignKey(pe => pe.IdEstudiante);
            });

            // ---- VISTA: vw_ReporteProyectos ----
            modelBuilder.Entity<ReporteProyectoView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_ReporteProyectos");
            });

            // ---- VISTA: vw_ReporteEstudiantes ----
            modelBuilder.Entity<ReporteEstudianteView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_ReporteEstudiantes");
            });
        }
    }
}
