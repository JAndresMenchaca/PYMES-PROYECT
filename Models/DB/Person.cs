using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class Person
{
    public int Id { get; set; }


    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras.")]
    [StringLength(49, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 2)]
    public string Name { get; set; } = null!;


    [Display(Name = "Apellido Paterno")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras.")]
    [StringLength(49, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 2)]
    public string LastName { get; set; } = null!;


    [Display(Name = "Apellido Materno")]
    [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras.")]
    [StringLength(49, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 2)]
    public string? SecondLastName { get; set; }


    [Display(Name = "Email")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(30, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 11)]
    [EmailAddress(ErrorMessage = "La dirección de correo electrónico no es válida.")]
    public string Email { get; set; } = null!;


    [Display(Name= "Numero de teléfono")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(12, ErrorMessage = "El campo {0} debe tener 8 caracteres.", MinimumLength = 8)]
    [RegularExpression(@"^\+?591([0-9]{8})$|^[0-9]{8}$", ErrorMessage = "El formato del campo {0} no es válido. Debe ser en formato +59169553766 o 69553766.")]
    public string PhoneNumber { get; set; } = null!;


    [Display(Name = "Genero")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Gender { get; set; } = null!;


    [Display(Name = "CI")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(11, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 7)]
    [RegularExpression(@"^\d{7,8}(-[A-Z]{1,2})?$", ErrorMessage = "El campo {0} debe tener el formato correcto, por ejemplo, 1293779 o 12937793-ET.")]
    public string Ci { get; set; } = null!;


    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public virtual BusinessManager? BusinessManager { get; set; }

    public virtual Producer? Producer { get; set; }

    public virtual User? User { get; set; }
}
