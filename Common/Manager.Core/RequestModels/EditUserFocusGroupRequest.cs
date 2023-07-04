namespace Manager.Core.RequestModels
{
    public class EditUserFocusGroupRequest
    {
        public Guid Id { get; set; }

        public string[] Grp { get; set; }
    }
}