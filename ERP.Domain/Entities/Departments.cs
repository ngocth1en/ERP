using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class Departments
{
    public string Id { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Assets_Information> Assets_InformationDepartment { get; set; } = new List<Assets_Information>();

    public virtual ICollection<Assets_Information> Assets_InformationUseDepartment { get; set; } = new List<Assets_Information>();

    public virtual ICollection<StorageLocations> StorageLocations { get; set; } = new List<StorageLocations>();

    public virtual ICollection<Users> Users { get; set; } = new List<Users>();
}
