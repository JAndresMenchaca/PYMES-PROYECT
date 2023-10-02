using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models
{
    public class Employee
    {
        public int? id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
