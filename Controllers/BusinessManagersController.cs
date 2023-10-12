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

namespace Proyecto_Pymes.Controllers
{
    public class BusinessManagersController : Controller
    {
        private readonly DbPymesContext _context;
        private string passw;

        public BusinessManagersController(DbPymesContext context)
        {
            _context = context;
        }

        // GET: BusinessManagers1
        public async Task<IActionResult> Index()
        {
            var dbPymesContext = _context.BusinessManagers.Include(b => b.IdEnterpriseNavigation).Include(b => b.IdNavigation).Where(b => b.Status == 1);
            return View(await dbPymesContext.ToListAsync());
        }

        // GET: BusinessManagers1/Details/5
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

        // GET: BusinessManagers1/Create
        public IActionResult Create()
        {
            ViewData["IdEnterprise"] = new SelectList(_context.Enterprises, "Id", "GroupName");
            return View();
        }

        // POST: BusinessManagers1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BusinessManager bs)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {



                        //registro para la persona-------------------------------------------------------------------------------------
                        _context.People.Add(bs.IdNavigation);
                        await _context.SaveChangesAsync();

                        string userID = HttpContext.Session.GetString("UserID");
                        bs.RegisterDate = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        bs.UserId = int.Parse(userID);
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
                        sendEmail(bs.IdNavigation.Email, user.UserName, passw, bs.IdNavigation.Name, bs.IdNavigation.LastName, user.Role);
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
        public async Task<IActionResult> Edit(BusinessManager bs)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Person per = bs.IdNavigation;
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
                            new SqlParameter("@PersonId", bs.Id)//OJO
                        };
                        int affectedRows = await _context.Database.ExecuteSqlRawAsync(sql, parameters);


                        //para la tabla Productor
                        string sql1 = @"UPDATE BusinessManager
                                    SET corporateNumber = @corporateNumber, lastUpdate = CURRENT_TIMESTAMP
                                    WHERE id = @BusinessManagerId;";
                        // Crea objetos SqlParameter para los parámetros de la consulta
                        SqlParameter[] parameters1 = new SqlParameter[]
                        {
                            new SqlParameter("@corporateNumber", bs.CorporateNumber),
                            new SqlParameter("@BusinessManagerId", bs.Id)
                        };
                        int affectedRows1 = await _context.Database.ExecuteSqlRawAsync(sql1, parameters1);

                        if (affectedRows > 0 && affectedRows1 > 0)
                        {
                            // Si todo ha ido bien, realiza el commit de la transacción
                            transaction.Commit();
                            TempData["ShowModal"] = true;
                        }

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


        // GET: BusinessManagers1/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.BusinessManagers == null)
        //    {
        //        return NotFound();
        //    }

        //    var businessManager = await _context.BusinessManagers.FindAsync(id);
        //    if (businessManager == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["IdEnterprise"] = new SelectList(_context.Enterprises, "Id", "Id", businessManager.IdEnterprise);
        //    ViewData["Id"] = new SelectList(_context.People, "Id", "Id", businessManager.Id);
        //    return View(businessManager);
        //}

        // POST: BusinessManagers1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // GET: BusinessManagers1/Delete/5
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
        public IActionResult UpdateShowModalDelete(bool value)
        {
            TempData["ShowModalDelete"] = value;
            return Json(new { success = true });
        }


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
            return RedirectToAction("Index");
        }

        private bool BusinessManagerExists(int id)
        {
          return (_context.BusinessManagers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private User Credentials(Person person)
        {
            User user = new User();
            string userName = (person.Name[0].ToString() + person.LastName[0].ToString() + person.SecondLastName[0].ToString()).ToUpper() + RandomNumber();
            user.Id = person.Id;
            user.UserName = userName;
            user.Password = PasswordGeneration(person);
            user.Role = "AdministradorEmpresa";
            user.UserId = 1;//OJO
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
        private byte[] PasswordGeneration(Person person)
        {
            using (MD5 md5 = MD5.Create())
            {
                string password = person.Ci.Substring(0, 5) + person.Email.Substring(0, 2);
                passw = password;
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return hashBytes;
            }
        }

        public int sendEmail(string email, string username, string password, string nombre, string apellido, string rol)
        {
            try
            {

                // Configurar los detalles del correo electrónico
                string remitente = "contacto.codensa@gmail.com";
                string destinatario = email;
                string asunto = "ENVIO DE CREDENCIALES A: " + nombre + " " + apellido;
                string cuerpoMensaje = "Estas son sus credenciales para ingresar al sistema, tenga mucho cuidado y no las comparta con nadie.\n" +
                                        "\nUsted esta registrado como un: " + rol +
                                        "\n\nNombre de usuario: " + username + "\n" +
                                        "\nContraseña: " + password + "\n" +
                                        "\nRecuerde que debera cambiar su contraseña al ingresar al sistema por primera vez" +
                                        "\n\nCualquier duda por favor ponganse en contacto con el administrador";

                // Crear el objeto MailMessage
                MailMessage correo = new MailMessage(remitente, destinatario, asunto, cuerpoMensaje);

                // Configurar el cliente SMTP
                SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com", 587);
                clienteSmtp.EnableSsl = true;
                clienteSmtp.UseDefaultCredentials = false;
                clienteSmtp.Credentials = new NetworkCredential("contacto.codensa@gmail.com", "wiabflozvurvzhhp");

                // Enviar el correo electrónico
                clienteSmtp.Send(correo);
                return 1;
            }
            catch
            {
                return 0;
            }
        }


    }
}
