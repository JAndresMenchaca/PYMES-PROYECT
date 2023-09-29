using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PymesDAO.Implementation
{
    public class BaseImpl
    {
        string connectionString = @"Server=DESKTOP-ADJ9ORU\SQLEXPRESS;Database=dbPymes;User Id=sa;Password=123;";
        internal string query = "";



        public SqlCommand CreateBasicCommand(string sql)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);
            return command;
        }

        public int ExecuteBasicCommand(SqlCommand command)
        {
            try
            {
                command.Connection.Open();//abrimos la connection
                return command.ExecuteNonQuery();//ejecutar la consulta
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { command.Connection.Close(); }
        }

        public DataTable ExecuteDataTable(SqlCommand command)
        {
            DataTable table = new DataTable();
            try
            {
                command.Connection.Open();//abrimos la connection
                SqlDataAdapter adapter = new SqlDataAdapter(command); //el resultado pasarlo al adaptador
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { command.Connection.Close(); }

            return table;
        }
    }
}
