using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;
using Proyecto_Pymes.ExtraModules;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_Pymes.Controllers
{
    [Authorize]
    public class BusinessManagersController : Controller
    {
        private readonly DbPymesContext _context;
        private string passw;
        CredentialGeneration credential;

        public BusinessManagersController(DbPymesContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Index()
        {
            var dbPymesContext = await _context.BusinessManagers.Include(b => b.IdEnterpriseNavigation).Include(b => b.IdNavigation).Where(b => b.Status != 0).ToListAsync();
            return View( dbPymesContext);
        }

        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BusinessManagers == null)
            {
                return NotFound();
            }

            var businessManager = await _context.BusinessManagers
                .Include(b => b.IdEnterpriseNavigation)
                .Include(b => b.IdNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessManager == null)
            {
                return NotFound();
            }

            return View(businessManager);
        }

        [Authorize(Roles = "SuperUsuario")]
        public IActionResult Create()
        {
            ViewData["IdEnterprise"] = new SelectList(_context.Enterprises.Where(e => e.Status == 1), "Id", "GroupName");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Create(BusinessManager bs)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        //registro para la persona-------------------------------------------------------------------------------------
                        var person =  _context.People.Add(bs.IdNavigation);
                        await _context.SaveChangesAsync();


                        //registro ala tabla Administrador
                        bs.Id = person.Entity.Id;
                        bs.UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                        _context.BusinessManagers.Add(bs);
                        await _context.SaveChangesAsync();


                        //registro para el Usuario----------------------------------------------------------------------------
                        User user = Credentials(bs.IdNavigation);
                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();

                        // Commit la transacción si todo fue exitoso
                        transaction.Commit();
                        //PARA EL MODAL
                        TempData["ShowModal"] = true;
                        //para el envio de credensiales
                        credential.sendEmail(bs.IdNavigation.Email, user.UserName, passw, bs.IdNavigation.Name, bs.IdNavigation.LastName, user.Role);
                    }
                }
                catch (Exception)
                {
                    // Si se produce una excepción, puedes realizar un rollback de la transacción
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error en la transacción.");
                }
            }
            return View(bs);
        }


        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Edit(int? id)
        {
           
            var businessManager = await _context.BusinessManagers.FindAsync(id);
            var people = await _context.People.FindAsync(id);

            if (businessManager == null || people == null || id == null)
            {
                return NotFound();
            }
            ViewData["IdEnterprise"] = new SelectList(_context.Enterprises, "Id", "GroupName", businessManager.IdEnterprise);
            return View(businessManager);
        }



        [HttpPost]
        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Edit(BusinessManager bs)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {                   
                          bs.LastUpdate = DateTime.Now;
                          bs.IdNavigation.LastUpdate = DateTime.Now;

                           _context.BusinessManagers.Update(bs);
                           await _context.SaveChangesAsync();
                        
                            transaction.Commit();
                            TempData["ShowModal"] = true;

                        return RedirectToAction("Edit", new { id = bs.Id }); // Redirecciona a la página de lista después de la edición
                    }
                    else
                    {
                        // Si hay errores en el modelo, no se realizará el commit de la transacción
                        return View(bs);
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                    return View(bs);
                }
            }
        }


        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> Delete(int? id)
        {
            var person = await _context.People.FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            TempData["BsToDelete"] = person.Id;
            TempData["Name"] = person.Name;
            TempData["lastName"] = person.LastName;
            TempData["SeconlastName"] = person.SecondLastName;
            TempData["ShowModalDelete"] = true;

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "SuperUsuario")]
        public IActionResult UpdateShowModalDelete(bool value)
        {
            TempData["ShowModalDelete"] = value;
            return Json(new { success = true });
        }


        [Authorize(Roles = "SuperUsuario")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var bs = await _context.BusinessManagers.FindAsync(id);
            if (bs == null)
            {
                return NotFound();
            }
            bs.Status = 0; // O es valor Inactivo          
            _context.BusinessManagers.Update(bs);
            await _context.SaveChangesAsync();


			var userd = await _context.Users
				.Include(p => p.IdNavigation)
				.FirstOrDefaultAsync(P => P.Id == bs.Id);

			if (userd == null)
			{
				return NotFound();
			}


			userd.Status = 0; // O es valor Inactivo          
			_context.Users.Update(userd);
			await _context.SaveChangesAsync();



			return RedirectToAction("Index");
        }

        private bool BusinessManagerExists(int id)
        {
          return (_context.BusinessManagers?.Any(e => e.Id == id)).GetValueOrDefault();
        }


       
        private User Credentials(Person person)
        {
            credential = new CredentialGeneration();
            User user = new User();
            //Credensiales
            string userName = (person.Name[0].ToString() + person.LastName[0].ToString() + RandomNumber());
            string password = person.Ci.Substring(0, 5) + person.Email.Substring(0, 2);
            passw = password;

            user.Id = person.Id;
            user.UserName = userName;
            user.Password = credential.PasswordEncryption(password);
            user.Role = "AdministradorEmpresa";
            user.UserId = int.Parse(HttpContext.Session.GetString("UserID"));
            return user;
        }


        private string RandomNumber()
        {
            string num = "";
            Random random = new Random();
            for (int i = 0; i < 7; i++)
            {
                int numeroAleatorio = random.Next(0, 9);
                num += numeroAleatorio;
            }
            return num;
        }

    }
}
