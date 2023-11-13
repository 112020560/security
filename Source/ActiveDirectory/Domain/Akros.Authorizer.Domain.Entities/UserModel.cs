namespace Akros.Authorizer.Domain.Entities
{
    public class UserModel
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Country { get; set; }
        public string? Ldap { get; set; }
    }
}
