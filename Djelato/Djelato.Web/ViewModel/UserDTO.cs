using Djelato.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Djelato.Web.ViewModel
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
