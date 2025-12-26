using ERP.Domain.Enum;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Filters
{
    public class PermissionRequiredAttribute : Attribute, IAuthorizationFilter
    {
        private readonly PermissionBit _bit;

        public PermissionRequiredAttribute(PermissionBit bit)
        {
            _bit = bit;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var perm = context.HttpContext.User.FindFirst("perm")?.Value;
            if (perm == null || perm[(int)_bit] != '1')
                context.Result = new ForbidResult();
        }
    }

}
