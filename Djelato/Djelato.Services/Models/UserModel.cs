using Djelato.Common.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.Services.Models
{
    public class UserModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public string Password { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
