using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Image
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public byte[] FilePath { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int IdProduct { get; set; }

    public int UserId { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;
}
