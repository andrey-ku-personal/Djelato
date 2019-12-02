using Djelato.Common.Entity;
using Microsoft.AspNetCore.Http;

namespace Djelato.Web.ViewModel
{
    public class UserDTO
    {
        public IFormFile Avatar { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public Role Role { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }        
    }
}
