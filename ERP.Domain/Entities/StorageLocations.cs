using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class StorageLocations
{
    public string StorageLocationNo { get; set; } = null!;

    public string NameEN { get; set; } = null!;

    public string NameVN { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;

    public int inUse { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Assets_Information> Assets_Information { get; set; } = new List<Assets_Information>();

    public virtual Departments Department { get; set; } = null!;
}
