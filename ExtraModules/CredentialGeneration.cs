using Proyecto_Pymes.Models.DB;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Proyecto_Pymes.ExtraModules
{
    public class CredentialGeneration
    {

        public byte[] PasswordEncryption(string password)
        {           
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return bytes;
        }


        //PARA EL ENVIO DE CREDENSIALES
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
