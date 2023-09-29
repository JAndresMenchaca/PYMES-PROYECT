using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models;
using Proyecto_Pymes.Models.DB;
using PymesDAO.Implementation;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;//para las secciones

namespace Proyecto_Pymes.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbPymesContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public LoginController(DbPymesContext context)
        {
            _context = context;
           

        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(Employee user)
        {
            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(user.Password);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    var userQuery = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName && u.Password == hashBytes);
                    if (userQuery != null)
                    {
                        //var personQuery = await _context.People.FirstOrDefaultAsync(p => p.Id == userQuery.Id);
                        //_contextAccessor.HttpContext.Session.SetString("UserName", personQuery.Name + " " + personQuery.LastName);
                        //_contextAccessor.HttpContext.Session.SetString("Rol", userQuery.Role);


                        //_contextAccessor.HttpContext.Session.SetString("EnterpriseID", "2");
                        HttpContext.Session.SetString("UserID", userQuery.Id.ToString());//OJO
                        HttpContext.Session.SetString("EnterpriseID", "2");//OJO----------------------------------------------
                        return RedirectToAction("Index", "Home");                   
                    }
                    else
                    {
                        ViewData["Error"] = "Credenciales incorrectas. Por favor, inténtelo nuevamente.";
                        return View();
                    }
                }                  
            }
            catch (System.Exception)
            {
                 ModelState.AddModelError(string.Empty, "Ocurrió un error durante el inicio de sesión");
                  return View(user);
            }
        }




        //UserImpl employee = new UserImpl();
        //DataTable table = employee.Login(user.UserName, user.Password);

        //if (table.Rows.Count > 0)
        //{
        //    ViewData["Error"] = "";
        //    return RedirectToAction("Index", "Home");
        //}
        //else
        //{
        //    ViewData["Error"] = "Credenciales incorrectas. Por favor, inténtelo nuevamente.";
        //    return View();
        //}
    }
}
