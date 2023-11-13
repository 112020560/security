using System.ComponentModel.DataAnnotations;

namespace Akros.Security.UserManager.Application.Dtos
{
    public class CreateUserDto
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? FullName { get; set; }
        [Required]
        public string[]? Roles { get; set; }
        [Required]
        public string? Email { get; set; }
        public bool IsActiveDirectory { get; set; }
        [Required]
        public string? Country { get; set; }
        [Required]
        public string? Company { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Supervisor { get; set; }
        //public Dictionary<string, string>? claims { get; set; }
    }
}
