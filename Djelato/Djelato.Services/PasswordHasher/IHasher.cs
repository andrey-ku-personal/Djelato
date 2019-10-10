using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.Services.PasswordHasher
{
    public interface IHasher
    {
        string HashPassword(string password, byte[] salt);
        byte[] GetSalt();
    }
}
