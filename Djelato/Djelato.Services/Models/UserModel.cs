using Djelato.Common.Entity;

namespace Djelato.Services.Models
{
    public class UserModel
    {
        public string AvatarPath { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
