namespace Akros.Authorizer.Domain.Entities
{
    public class CreateAdUserModel
    {
        public string Country { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? OfficeName { get; set; }
        public string?  PhoneNumber { get; set; }
        public string? Identification { get; set; }
        
        public AccessProperties UserManagerAccess { get; set; }

    }

    public class AccessProperties
    {
        public string? LDap { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
