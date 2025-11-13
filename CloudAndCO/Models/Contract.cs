using System;
using System.Collections.Generic;

namespace CloudAndCO.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public int CustomerId { get; set; }

    public string ContractUrl { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
