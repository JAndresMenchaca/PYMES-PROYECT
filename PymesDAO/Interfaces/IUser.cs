using PymesDAO.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PymesDAO.Interfaces
{
    public interface IUser:IBase<User>
    {
        DataTable Login(string userName, string password);
    }
}
