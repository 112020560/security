using System;

namespace Akros.Authorizer.Application.Dto
{
    public class UserLoggedDto
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Country { get; set; }
    }
}
