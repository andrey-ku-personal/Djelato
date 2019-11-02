using Djelato.Services.PasswordHasher;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Djelato.xUnitTests.Service
{
    public class HasherTests
    {
        [Fact]
        public void Should_ReturnNewSalt_When_CallMethodAsync()
        {
            //Arrange
            Hasher hasher = new Hasher();

            //Action
            var resultSalt = hasher.GetSalt();

            //Assert
            Assert.IsType<byte[]>(resultSalt);
            Assert.True(resultSalt.Length == Hasher.SaltByteSize);
        }

        [Fact]
        public void Should_ReturnHash_When_HashPasswordAsync()
        {
            //Arrange
            string pass = "1234567A";
            Hasher hasher = new Hasher();
            var resultSalt = hasher.GetSalt();

            //Action
            var resultHash1 = hasher.HashPassword(pass, resultSalt);
            var resultHash2 = hasher.HashPassword(pass, resultSalt);

            //Assert
            Assert.IsType<string>(resultHash1);
            Assert.IsType<string>(resultHash2);
            Assert.True(resultHash1 == resultHash2);
        }
    }
}
