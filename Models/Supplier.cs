using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Supplier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? SecondLastName { get; set; }

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public string? UserId { get; set; }

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
