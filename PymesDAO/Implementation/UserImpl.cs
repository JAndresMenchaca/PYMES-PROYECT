using PymesDAO.Interfaces;
using PymesDAO.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PymesDAO.Implementation
{
    public class UserImpl : BaseImpl, IUser
    {
        public int Delete(User t)
        {
            throw new NotImplementedException();
        }

        public int Insert(User t)
        {
            throw new NotImplementedException();
        }

        public DataTable Login(string userName, string password)
        {
         

            query = @"SELECT id, userName, password, role , firstLogin 
                      FROM [User]
                      WHERE status = 1 AND userName = @username AND password = HASHBYTES('MD5',@password)";
            SqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@username", userName);
            command.Parameters.AddWithValue("@password", password).SqlDbType = SqlDbType.VarChar;
            try
            {
                return ExecuteDataTable(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { command.Dispose(); }
        }

        public DataTable Select()
        {
            throw new NotImplementedException();
        }

        public int Update(User t)
        {
            throw new NotImplementedException();
        }
    }
}
