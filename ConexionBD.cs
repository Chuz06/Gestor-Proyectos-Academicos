using Microsoft.Data.SqlClient;

namespace Gestor_Proyectos_Academicos
{
    public class ConexionBD
    {
        private SqlConnection conexion;

        
        public ConexionBD()
        {
            
            string cadena = "Server=localhost;Database=GestorProyectos; Integrated Security=True; Encrypt=False;";
            conexion = new SqlConnection(cadena);
        }

       
        public void Abrir()
        {
            if (conexion.State == System.Data.ConnectionState.Closed)
            {
                conexion.Open();
            }
        }

       
        public void Cerrar()
        {
            if (conexion.State == System.Data.ConnectionState.Open)
            {
                conexion.Close();
            }
        }

        
        public SqlConnection ObtenerConexion()
        {
            return conexion;
        }

    }
}
