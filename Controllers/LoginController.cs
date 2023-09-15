using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Models;
using WebApplication1.Models.ModelForm;


namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbPymesContext  _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public LoginController(DbPymesContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _contextAccessor = httpContextAccessor;

        }
      
        public IActionResult Index()
        {
            return View();
        }
        //para el Inicio de Seccion
        [HttpPost]
        public async Task<IActionResult> Index(UserForm user)
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
                        var personQuery = await _context.People.FirstOrDefaultAsync(p => p.Id == userQuery.Id);
                        _contextAccessor.HttpContext.Session.SetString("UserName", personQuery.Name+" "+personQuery.LastName);
                        _contextAccessor.HttpContext.Session.SetString("Rol", userQuery.Role);                                  
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


      


    }
}
