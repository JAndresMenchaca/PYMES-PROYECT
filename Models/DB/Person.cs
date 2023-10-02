using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class Person
{
    
    public int Id { get; set; }
	[Required(ErrorMessage = "El Nombre es Obligatorio")]
	public string Name { get; set; } = null!;

	[Required(ErrorMessage = "El Apellido es Obligatorio")]
	public string LastName { get; set; } = null!;

    public string? SecondLastName { get; set; }

	[Required(ErrorMessage = "El Gmail es Obligatorio")]
	public string Email { get; set; } = null!;

	[Required(ErrorMessage = "El Numero de Telefono es Obligatorio")]
	public string PhoneNumber { get; set; } = null!;

	[Required(ErrorMessage = "El genero es Obligatorio")]
	public string Gender { get; set; } = null!;

	[Required(ErrorMessage = "El CI  es Obligatorio")]
    public string Ci { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public virtual BusinessManager? BusinessManager { get; set; }

    public virtual Producer? Producer { get; set; }

    public virtual User? User { get; set; }
}
