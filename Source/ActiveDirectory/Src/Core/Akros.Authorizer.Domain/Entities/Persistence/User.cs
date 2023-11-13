namespace Akros.Authorizer.Domain.Entities.Persistence
{
    public sealed record User
    {
        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string[]? LDAPs { get; set; }
        public string? LDAP { get; set; }
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
    }
}
