using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models;
using Proyecto_Pymes.Models.DB;
using Microsoft.AspNetCore.Http;//para las secciones
using Proyecto_Pymes.ExtraModules;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace Proyecto_Pymes.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbPymesContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        CredentialGeneration credential;
        public bool band = false;

        public LoginController(DbPymesContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(User user, string Password)
        {
            try
            {

                               
                    var userQuery = await _context.Users
                                   .Include(p => p.IdNavigation)
                                   .FirstOrDefaultAsync(u => u.UserName == user.UserName);



                    if (userQuery != null)
                    {
                        //para poder encriptar la contrasenia
                        credential = new CredentialGeneration();
                        byte[] pass = credential.PasswordEncryption(Password);
                   		

					if (userQuery.Password.SequenceEqual(pass))
                        {

                        if(userQuery.Status == 0)
                        {
							TempData["Password"] = "Usted no tiene acceso al sistema";
							return View();
						}

                        if(userQuery.Role == "AdministradorEmpresa")
                        {
                            var BQuery = await _context.BusinessManagers
                                   .Include(p => p.IdNavigation)
                                   .FirstOrDefaultAsync(u => u.Id == userQuery.Id);

                            if (BQuery.Status == 2)
                            {
                                TempData["Password"] = "Su cuenta esta deshabilitada, contacte al adminstrador";
                                return View();
                            }
                        }

						//bariables de session en comun
						HttpContext.Session.SetString("UserID", userQuery.Id.ToString());
                        HttpContext.Session.SetString("role", userQuery.Role.ToString());
                        HttpContext.Session.SetString("FullName", userQuery.IdNavigation.Name + "-" + userQuery.IdNavigation.LastName);

                            //para poder recuperar algunos credensiales extra
                            if (userQuery.Role == "AdministradorEmpresa")
                            {
                                var adminEnterprise = await _context.BusinessManagers
                                    .Include(en => en.IdEnterpriseNavigation)
                                    .FirstOrDefaultAsync(p => p.Id == userQuery.Id);
                                if (adminEnterprise != null && adminEnterprise.IdEnterpriseNavigation != null)
                                {
                                    HttpContext.Session.SetString("EnterpriseID", adminEnterprise.IdEnterprise.ToString());
                                    HttpContext.Session.SetString("NameEnterprise", adminEnterprise.IdEnterpriseNavigation.GroupName);
                                }
                            }


                              //----------------------------------------------------------------------------
                              var claims = new List<Claim>()
                               {
                                   new Claim(ClaimTypes.Name, userQuery.IdNavigation.Name),
                                   new Claim("UserId", userQuery.Id.ToString()),
                                   new Claim(ClaimTypes.Role, userQuery.Role)
                               };
                              
                              var claimIdentity = new ClaimsIdentity(claims,
                                  CookieAuthenticationDefaults.AuthenticationScheme);
                              
                              await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                  new ClaimsPrincipal(claimIdentity)
                              );
                              //-----------------------------------------------------------------------------
     
                              return RedirectToAction("Index", "Home");
                        }
                        else
                        {                                                
                            TempData["Password"] = "La contraseña es incorrecta. Por favor, inténtelo nuevamente.";
                        }                     
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "El nombre de usuario no existe. Por favor, inténtelo nuevamente.");                    
                    }
                                 
            }
            catch (System.Exception)
            {
                 ModelState.AddModelError(string.Empty, "Ocurrió un error durante el inicio de sesión");         
            }
           
            return View(user);
        }


        public async Task<IActionResult> Logout(User userF)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

    }
}
