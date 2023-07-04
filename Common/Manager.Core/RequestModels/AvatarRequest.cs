using Manager.Core.Page;

namespace Manager.Core.RequestModels
{
    public class AvatarRequest : QueryParameters
    {
        public Guid UId { get; set; }
    }
}