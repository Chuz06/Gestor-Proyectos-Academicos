Gestor de Proyectos Académicos

Sistema web para la gestión de proyectos universitarios, permitiendo a profesores y estudiantes administrar tareas, roles, reportes y avances dentro de un entorno académico estructurado.

 Descripción breve

El Gestor de Proyectos Académicos es un sistema desarrollado en ASP.NET Core MVC con conexión a SQL Server, que permite:

Administración de usuarios (profesores y estudiantes).

Creación y asignación de proyectos.

Gestión de tareas con estados.

Autenticación con hashing y control de permisos por rol.

Reportes automáticos para profesores y estudiantes.

Dashboards dinámicos según el rol del usuario.

 Requisitos técnicos
Lenguaje y Framework

C#

.NET 8 / ASP.NET Core MVC

Entity Framework Core

Razor Views

Base de datos

Microsoft SQL Server

SQL Server Management Studio (SSMS) recomendado

Librerías / Dependencias

Microsoft.Data.SqlClient

EntityFrameworkCore.SqlServer

EntityFrameworkCore.Tools

Autenticación por Cookies

Hashing SHA256 para contraseñas

- Instrucciones de instalación
1️- Clonar el repositorio
git clone https://github.com/Gestor-Proyectos-Academicos.git


Luego abre el proyecto con Visual Studio 2022.

2️- Restaurar la base de datos

Abrir SQL Server Management Studio.

Crear una BD llamada:

GestorProyectos


Ejecutar el script completo del proyecto (tablas, vistas, datos iniciales).

 El script se encuentra en la carpeta:

/BaseDeDatos/Script.sql

3️-Configurar la conexión a la base de datos

En appsettings.json, modificar:

"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=GestorProyectos;Trusted_Connection=True;TrustServerCertificate=True;"
}

4️- Ejecutar el proyecto

En Visual Studio:

Seleccionar IIS Express / Kestrel

Presionar F5

El sistema abrirá automáticamente la pantalla de Login.

- Guía rápida de uso
 1. Iniciar sesión

El sistema requiere credenciales válidas:

Profesor: acceso a proyectos, asignaciones y reportes globales.

Estudiante: acceso solo a sus tareas y reportes personales.

 2. Crear un proyecto (Profesor)

Entrar al módulo “Proyectos”.

Seleccionar Nuevo Proyecto.

Ingresar nombre, descripción, fechas y crear.

El profesor logueado se asocia automáticamente.

 3. Crear una tarea (Profesor o Estudiante según permisos)

Abrir un proyecto.

Seleccionar Agregar Tarea.

Elegir estudiante asignado, fecha límite y descripción.

Guardar.

 4. Ver reportes

Profesor:

Avance por proyecto

Cantidad de tareas completadas

Progreso por estudiante

Estudiante:

Avance personal

Estado de sus tareas

Licencia

Este proyecto puede usar la licencia:

MIT License


Permite uso libre, modificación y distribución con créditos al autor.



Equipo de desarrollo:

Luis Quiroz

Jesús Solis

Ariela Rivera
