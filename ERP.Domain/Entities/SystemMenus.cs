using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class SystemMenus
{
    public string Code { get; set; } = null!;

    public string SystemCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Route { get; set; }

    public int OrderNo { get; set; }

    public bool IsUsing { get; set; }

    public string? ParentCode { get; set; }

    public virtual ICollection<SystemMenus> InverseParentCodeNavigation { get; set; } = new List<SystemMenus>();

    public virtual SystemMenus? ParentCodeNavigation { get; set; }

    public virtual SystemModules SystemCodeNavigation { get; set; } = null!;

    public virtual ICollection<UserMenuPermissions> UserMenuPermissions { get; set; } = new List<UserMenuPermissions>();
}
