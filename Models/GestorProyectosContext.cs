using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Gestor_Proyectos_Academicos.Models
{
    public class GestorProyectosContext : DbContext
    {
        public GestorProyectosContext(DbContextOptions<GestorProyectosContext> options)
            : base(options)
        {
        }

        // Tablas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<ProyectosEstudiantes> ProyectosEstudiantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---- USUARIOS / ROLES (como ya lo tenías) ----
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.IdUsuario);

                entity.Property(u => u.IdRol)
                      .HasColumnName("IdRol");

                entity.HasOne(u => u.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(u => u.IdRol);
            });

            // ---- PROYECTOS-ESTUDIANTES ----
            modelBuilder.Entity<ProyectosEstudiantes>(entity =>
            {
                entity.HasKey(pe => new { pe.IdProyecto, pe.IdEstudiante });

                entity.HasOne(pe => pe.Proyecto)
                      .WithMany()                      // si quieres, luego le agregamos colección en Proyecto
                      .HasForeignKey(pe => pe.IdProyecto);

                entity.HasOne(pe => pe.Estudiante)
                      .WithMany()                      // luego se puede refinar (solo estudiantes)
                      .HasForeignKey(pe => pe.IdEstudiante);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
