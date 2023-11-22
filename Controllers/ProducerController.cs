using Microsoft.AspNetCore.Mvc;
using Proyecto_Pymes.Models.DB;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Net.Mail;
using System.Net;
using Proyecto_Pymes.ExtraModules;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_Pymes.Controllers
{
    [Authorize]
    public class ProducerController : Controller
    {

        private readonly DbPymesContext _context;
        private string passw;
        CredentialGeneration credential;

        public ProducerController(DbPymesContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "AdministradorEmpresa")]
        public async Task<IActionResult> Index()
        {
          
            try{
            int enterpriseId = int.Parse( HttpContext.Session.GetString("EnterpriseID"));      
            var producer = await _context.Producers
                         .Include(pr => pr.IdNavigation) // Mueve la llamada a Include antes de Select
                         .Join(
                             _context.ProducerCompanies,
                             p => p.Id,
                             pc => pc.IdProducer,
                             (p, pc) => new { Producer = p, ProducerCompany = pc }
                         )
                         .Where(e => e.ProducerCompany.Status == 1 && e.ProducerCompany.IdEnterprise == enterpriseId)                
                         .Select(e => e.Producer) // Proyecta solo la entidad Producer
                         .ToListAsync();

            return View(producer);
            }
            catch{
                return RedirectToAction("Privacy", "Home");
            }
        }


        [Authorize(Roles = "AdministradorEmpresa")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "AdministradorEmpresa")]
        public async Task<IActionResult> Create(Producer producer)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        //registro para la persona-------------------------------------------------------------------------------------
                        var person = await  _context.People.AddAsync(producer.IdNavigation);
                        await _context.SaveChangesAsync();

                        //registro para el Productor---------------------------------------------------------------------------------
                      

                        producer.Id = person.Entity.Id;            
                        producer.UserId = int.Parse(HttpContext.Session.GetString("UserID"));
                        await  _context.Producers.AddAsync(producer);
                        await _context.SaveChangesAsync();

                        //registro para el Usuario----------------------------------------------------------------------------
                        User user = Credentials(producer.IdNavigation);
                        await _context.Users.AddAsync(user);
                        await _context.SaveChangesAsync();


                        //registro para la tabla de muchos a muchos---------------------------------------------------------------------
                        ProducerCompany company= new ProducerCompany();                      
                        company.IdProducer = person.Entity.Id;
                        company.IdEnterprise = short.Parse(HttpContext.Session.GetString("EnterpriseID"));
                        company.StartDate = DateTime.Now;

                        await _context.ProducerCompanies.AddAsync(company);
                        await _context.SaveChangesAsync();

                        // Commit la transacción si todo fue exitoso
                        transaction.Commit();
                        //PARA EL MODAL
                        TempData["ShowModal"] = true;
                        credential.sendEmail(producer.IdNavigation.Email, user.UserName, passw, producer.IdNavigation.Name, producer.IdNavigation.LastName, user.Role);
                    }
                }
                catch (Exception)
                {
                    // Si se produce una excepción, puedes realizar un rollback de la transacción
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error en la transacción.");
                }
            }
            return View();
        }




        private User Credentials(Person person)
        {
            credential = new CredentialGeneration();
            User user = new User();
            string userName = (person.Name[0].ToString() + person.LastName[0].ToString() + RandomNumber());
            string password = person.Ci.Substring(0, 5) + person.Email.Substring(0, 2);
            passw = password;


            user.Id = person.Id;
            user.UserName = userName;
            user.Password = credential.PasswordEncryption(password);
            user.Role = "Productor";
            user.UserId = int.Parse(HttpContext.Session.GetString("UserID"));

            return user;
        }

        private string RandomNumber()
        {
            string num = "";
            Random random = new Random();
            for(int i = 0; i<7 ; i++)
            {
                int numeroAleatorio = random.Next(0, 9);
                num += numeroAleatorio;
            }
            return num;
        }



        [Authorize(Roles = "AdministradorEmpresa")]
        public async Task<IActionResult> Edit(int? id)
        {        
            var prod = await _context.Producers.FindAsync(id);
            var people = await _context.People.FindAsync(id);
            
            if (prod == null || people == null || id == null)
            {
                return NotFound();
            }

            return View(prod);
        }




        [HttpPost]
        [Authorize(Roles = "AdministradorEmpresa")]
        public async Task<IActionResult> Edit(Producer producer)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                            producer.LastUpdate = DateTime.Now;
                            producer.IdNavigation.LastUpdate = DateTime.Now;

                            _context.Producers.Update(producer);
                            await _context.SaveChangesAsync();
                            // Si todo ha ido bien, realiza el commit de la transacción
                            transaction.Commit();
                            TempData["ShowModal"] = true;
                       // }

                        return RedirectToAction("Edit", new { id = producer.Id }); // Redirecciona a la página de lista después de la edición
                    }
                    else
                    {
                        // Si hay errores en el modelo, no se realizará el commit de la transacción
                        return View(producer);
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                    return View(producer);
                }
            }
        }



        [Authorize(Roles = "AdministradorEmpresa")]
        public async Task<IActionResult> Delete(int? id)
        {
            var producer = await _context.Producers
                .Include(p => p.IdNavigation)
                .FirstOrDefaultAsync(P => P.Id == id);

			if (producer == null)
            {
                return NotFound();
            }

			return PartialView("DeleteViewPartial", producer);
        }

        [Authorize(Roles = "AdministradorEmpresa")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            int enterpriseId = int.Parse(HttpContext.Session.GetString("EnterpriseID"));
            if (id == null)
            {
                return NotFound();
            }
            var producer = await _context.ProducerCompanies.Where(p => p.IdProducer == id && p.IdEnterprise == enterpriseId && p.Status == 1).FirstAsync();
            if (producer == null)
            {
                return NotFound();
            }   
            producer.Status = 0; // O es valor Inactivo          
            _context.ProducerCompanies.Update(producer);
            await _context.SaveChangesAsync();

			var userd = await _context.Users
				.Include(p => p.IdNavigation)
				.FirstOrDefaultAsync(P => P.Id == producer.IdProducer);

			if (userd == null)
			{
				return NotFound();
			}


			userd.Status = 0; // O es valor Inactivo          
			_context.Users.Update(userd);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
        }
    }
}
