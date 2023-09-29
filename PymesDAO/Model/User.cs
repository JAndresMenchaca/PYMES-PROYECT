using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PymesDAO.Model
{
    public class User
    {
        public int id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        public byte FirstLogin { get; set; }
        public int userID { get; set; }

        public User(int id, string username, string password, string rol, byte firstLogin, int userID)
        {
            this.id = id;
            Username = username;
            Password = password;
            Rol = rol;
            FirstLogin = firstLogin;
            this.userID = userID;
        }
    }
}
