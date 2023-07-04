using Manager.Core.Models.Accounts;
using Manager.Core.Models.Modules;

namespace Manager.Core.AuthorizationModels
{
    public class RolePermissionViewModel
    {
        public List<ModuleInfo>? ModuleInfos { get; set; }

        public List<RolePermission>? RolePermissions { get; set; }
    }
}