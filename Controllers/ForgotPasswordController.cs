using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models;
using Proyecto_Pymes.Models.DB;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyecto_Pymes.Models.DB;
using System.Reflection.Metadata;
using System.Net.Mail;
using System.Net;
using Proyecto_Pymes.ExtraModules;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Proyecto_Pymes.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly DbPymesContext _context;

        public Person personaEncontrada {get; set;}
        private string passw;
        CredentialGeneration credential;

        public ForgotPasswordController(DbPymesContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult newPassw()
        {

            return View();
        }


        public async Task<IActionResult> updatePasswAsync(string? email, int? code)
        {
            if (email != null)
            {
                personaEncontrada = _context.People.FirstOrDefault(p => p.Email == email);

                HttpContext.Session.SetInt32("idPassw", personaEncontrada.Id);

                Random rand = new Random();
                int num = rand.Next(10000, 99999);

                HttpContext.Session.SetInt32("codeUser", num);

                sendEmail(personaEncontrada.Email, personaEncontrada.Name, personaEncontrada.LastName, num);

                return View();

            }
            else if(code != null)
            {
                if (code == HttpContext.Session.GetInt32("codeUser"))
                {
                    personaEncontrada = _context.People.FirstOrDefault(p => p.Id == HttpContext.Session.GetInt32("idPassw"));

                    var usuarioEncontrado = _context.Users
                    .Where(u => u.Id == personaEncontrada.Id)
                    .FirstOrDefault();

                    string password = personaEncontrada.Ci.Substring(0, 5) + personaEncontrada.Email.Substring(0, 2);
                    passw = password;
                    credential = new CredentialGeneration();
                    usuarioEncontrado.Password = credential.PasswordEncryption(password);
                    usuarioEncontrado.UserId = personaEncontrada.Id;

                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            if (ModelState.IsValid)
                            {
                                //para la tabla Productor
                                string sql1 = @" UPDATE [User] SET password = @passw, lastUpdate = CURRENT_TIMESTAMP, userID = @userId WHERE id = @userId";
                                // Crea objetos SqlParameter para los parámetros de la consulta
                                SqlParameter[] parameters1 = new SqlParameter[]
                                {
                                    new SqlParameter("@passw", usuarioEncontrado.Password)          ,
                                    new SqlParameter("@userId", usuarioEncontrado.Id)
                                };
                                int affectedRows1 = await _context.Database.ExecuteSqlRawAsync(sql1, parameters1);

                                if (affectedRows1 > 0)
                                {
                                    // Si todo ha ido bien, realiza el commit de la transacción
                                    transaction.Commit();
                                    sendEmail1(personaEncontrada.Email, usuarioEncontrado.UserName, passw, personaEncontrada.Name, personaEncontrada.LastName, usuarioEncontrado.Role);                                    return RedirectToAction("newPassw");
                                    return RedirectToAction("newPassw");
                                }

                                return View(); // Redirecciona a la página de lista después de la edición
                            }
                            else
                            {
                                // Si hay errores en el modelo, no se realizará el commit de la transacción
                                return View();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Si ocurre un error, puedes manejarlo aquí y realizar un rollback de la transacción si es necesario
                            transaction.Rollback();
                            ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
                            return View();
                        }
                    }

                    
                }

                return View();
            }

            return View();
        }

     


        [HttpPost]
        public async Task<IActionResult> Index(string email)
        {
            try
            {
                var personaExistente = _context.People.FirstOrDefault(p => p.Email == email);

                if (personaExistente != null)
                {
                    return RedirectToAction("updatePassw", new { email = email }); ;
                }
                else
                {
                    // El correo electrónico proporcionado no corresponde a ningún registro en la tabla de Personas.
                    TempData["NotEmail"] = "No se encontró ninguna coincidencia para el correo electrónico proporcionado.";
                    ViewBag.Email = email;
                }
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error durante el inicio de sesión");
            }
          
            return View();
        }

        public int sendEmail(string email, string nombre, string apellido, int num)
        {
            try
            {

                // Configurar los detalles del correo electrónico
                string remitente = "contacto.codensa@gmail.com";
                string destinatario = email;
                string asunto = "ENVIO DE CREDENCIALES A: " + nombre + " " + apellido;
                string cuerpoMensaje = "Este es un codigo para reestablecer la contraseña, tenga mucho cuidado y no lo comparta con nadie.\n" +
                                        "\n el codigo es: " + num +
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

        public int sendEmail1(string email, string username, string password, string nombre, string apellido, string rol)
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
