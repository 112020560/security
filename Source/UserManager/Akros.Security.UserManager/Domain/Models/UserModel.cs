namespace Akros.Security.UserManager.Domain.Models
{
    public class UserModel
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public string? Country { get; set; }
        public bool ActiveDirectory { get; set; }
        public string? Company { get; set; }
        public List<string>? Roles { get; set; }
        public string? Supervisor { get; set; }
    }
}
