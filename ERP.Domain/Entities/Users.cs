using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class Users
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;

    public string? Address { get; set; }

    public string? BankName { get; set; }

    public string? BankNum { get; set; }

    public string? Email { get; set; }

    public string? FName { get; set; }

    public string? LName { get; set; }

    public bool MustChangePassword { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual Departments Department { get; set; } = null!;

    public virtual ICollection<UserMenuPermissions> UserMenuPermissions { get; set; } = new List<UserMenuPermissions>();

    public virtual ICollection<UserSystems> UserSystems { get; set; } = new List<UserSystems>();
}
