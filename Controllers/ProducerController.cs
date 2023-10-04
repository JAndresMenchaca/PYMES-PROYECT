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

namespace Proyecto_Pymes.Controllers
{
    public class ProducerController : Controller
    {

        private readonly DbPymesContext _context;

        public ProducerController(DbPymesContext context)
        {
            _context = context;
        }
        //para poder recuperar los datos
        public async Task<IActionResult> Index()
        {
            //var sql = @"SELECT *
            //            FROM Person
            //            WHERE id IN (
            //                SELECT pr.id
            //                FROM Enterprise e
            //                INNER JOIN producerCompany pc ON e.id = pc.idEnterprise
            //                INNER JOIN Producer pr ON pr.id = pc.idProducer
            //                WHERE pr.status = 1 AND e.id = @EnterpriseId )";
            ////recuperamos el id de la empresa don de se filtrar los productores
            int enterpriseId = int.Parse( HttpContext.Session.GetString("EnterpriseID"));      
            var producer = await _context.Producers
                         .Include(pr => pr.IdNavigation) // Mueve la llamada a Include antes de Select
                         .Join(
                             _context.ProducerCompanies,
                             p => p.Id,
                             pc => pc.IdProducer,
                             (p, pc) => new { Producer = p, ProducerCompany = pc }
                         )
                         .Where(e => e.Producer.Status == 1 && e.ProducerCompany.IdEnterprise == enterpriseId)                
                         .Select(e => e.Producer) // Proyecta solo la entidad Producer
                         .ToListAsync();

            return View(producer);
        }


        /// <summary>
        /// para el Insert de Productor------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <returns></returns>
        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producer producer)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //registro para la persona-------------------------------------------------------------------------------------
                        _context.People.Add(producer.IdNavigation);
                        await _context.SaveChangesAsync();

                        //registro para el Productor---------------------------------------------------------------------------------
                        int personId = producer.IdNavigation.Id;
                        producer.Id = personId;//OJO------------------------------------
                        string userID = HttpContext.Session.GetString("UserID");
                        producer.UserId = int.Parse(userID);
                        _context.Producers.Add(producer);
                        await _context.SaveChangesAsync();

                        //registro para el Usuario----------------------------------------------------------------------------
                        User user = Credentials(producer.IdNavigation);
                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();

                        //registro para la tabla de muchos a muchos---------------------------------------------------------------------
                        ProducerCompany company= new ProducerCompany();
                        string enterpriseId = HttpContext.Session.GetString("EnterpriseID");
                        company.IdProducer = personId;
                        company.IdEnterprise = short.Parse(enterpriseId);//OJO----------------------------------
                        company.StartDate = DateTime.Now;
                        _context.ProducerCompanies.Add(company);
                        await _context.SaveChangesAsync();

                        // Commit la transacción si todo fue exitoso
                        transaction.Commit();
                        //PARA EL MODAL
                        TempData["ShowModal"] = true;
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
            User user = new User();
            string userName = (person.Name[0].ToString() + person.LastName[0].ToString() + person.SecondLastName[0].ToString()).ToUpper() + RandomNumber();
            user.Id = person.Id;
            user.UserName = userName;
            user.Password = PasswordGeneration(person);
            user.Role = "Productor";
            user.UserId = 1;//OJO
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
        private byte[] PasswordGeneration(Person person)
        {
            using (MD5 md5 = MD5.Create())
            {
                string password = person.Ci.Substring(0, 5) + person.Email.Substring(0,2);
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return hashBytes;
            }
        }


        //para poder Editar al Productor
        // GET: People/Create
        // GET: People/Delete/5

        // GET: People/Edit/5
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
        public async Task<IActionResult> Edit(Producer producer)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Person per = producer.IdNavigation;
                        string sql = @"UPDATE Person
                                    SET name = @Name, lastName = @LastName, secondLastName = @SecondLastName, email = @Email, phoneNumber = @PhoneNumber, gender = @Gender, ci = @CI ,lastUpdate = CURRENT_TIMESTAMP
                                    WHERE id = @PersonId; ";

                        // Crea objetos SqlParameter para los parámetros de la consulta
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@Name", per.Name),
                            new SqlParameter("@LastName", per.LastName),
                            new SqlParameter("@SecondLastName", per.SecondLastName),
                            new SqlParameter("@Email", per.Email),
                            new SqlParameter("@PhoneNumber", per.PhoneNumber),
                            new SqlParameter("@Gender", per.Gender),
                            new SqlParameter("@CI", per.Ci),
                            new SqlParameter("@PersonId", producer.Id)//OJO
                        };
                        int affectedRows = await _context.Database.ExecuteSqlRawAsync(sql, parameters);


                        //para la tabla Productor
                        string sql1 = @"UPDATE Producer
                                    SET longitude = @Longitude, latitude = @Latitude, lastUpdate = CURRENT_TIMESTAMP
                                    WHERE id = @ProducerId;";
                        // Crea objetos SqlParameter para los parámetros de la consulta
                        SqlParameter[] parameters1 = new SqlParameter[]
                        {
                            new SqlParameter("@Longitude", producer.Longitude),
                            new SqlParameter("@Latitude", producer.Latitude),
                            new SqlParameter("@ProducerId", producer.Id)
                        };
                        int affectedRows1 = await _context.Database.ExecuteSqlRawAsync(sql1, parameters1);

                        if (affectedRows > 0 && affectedRows1 > 0)
                        {
                            // Si todo ha ido bien, realiza el commit de la transacción
                            transaction.Commit();
                            TempData["ShowModal"] = true;
                        }

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



        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var producer = await _context.Producers.FirstOrDefaultAsync(P => P.Id == id);                          
            if (producer == null)
            {
                return NotFound();
            }
          
            return PartialView("DeleteViewPartial", producer);
        }

        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var producer = await _context.Producers.FindAsync(id);
            if (producer == null)
            {
                return NotFound();
            }   
            producer.Status = 0; // O es valor Inactivo          
            _context.Producers.Update(producer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
