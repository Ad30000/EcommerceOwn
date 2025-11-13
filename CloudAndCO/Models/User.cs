using System;
using System.Collections.Generic;

namespace CloudAndCO.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
