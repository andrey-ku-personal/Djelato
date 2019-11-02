using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Djelato.Services.PasswordHasher
{
    public class Hasher : IHasher
    {
        public const int SaltByteSize = 16;
        public const int Pbkdf2Iterations = 10000;
        public const int Pbkdf2Index = 2;
        public const int DesiredLength = 32;

        public string HashPassword(string password, byte[] salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: Pbkdf2Iterations,
                numBytesRequested: DesiredLength));

            return hashed;
        }

        public byte[] GetSalt()
        {
            RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SaltByteSize];
            cryptoProvider.GetBytes(salt);

            return salt;
        }
    }
}
