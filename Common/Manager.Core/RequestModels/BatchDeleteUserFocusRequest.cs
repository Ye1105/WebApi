namespace Manager.Core.RequestModels
{
    public class BatchDeleteUserFocusRequest
    {
        public Guid[] Ids { get; set; }
        public Guid UId { get; set; }
    }
}