using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class SystemModules
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<SystemGuideFiles> SystemGuideFiles { get; set; } = new List<SystemGuideFiles>();

    public virtual ICollection<SystemMenus> SystemMenus { get; set; } = new List<SystemMenus>();

    public virtual ICollection<UserSystems> UserSystems { get; set; } = new List<UserSystems>();
}
