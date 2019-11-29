using Djelato.DataAccess.Entity;
using Djelato.DataAccess.Managers.Interfaces;
using Djelato.Services.JWT;
using Djelato.Services.JWT.Interfaces;
using Djelato.Services.Models;
using Djelato.Services.PasswordHasher;
using Djelato.Services.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Djelato.xUnitTests.Service
{
    public class AuthServiceTests
    {
        private readonly Mock<IMongoManager> _mockMongoManager;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly Mock<IHasher> _mockHasher;
        private readonly Mock<IJwtSigningEncodingKey> _mockSignEncodingKey;
        private readonly Mock<IJwtEncryptingEncodingKey> _mockEncryptEncodingKey;

        private readonly AuthModel _authModel;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockMongoManager = new Mock<IMongoManager>();
            _mockLogger = new Mock<ILogger<AuthService>>();
            _mockHasher = new Mock<IHasher>();
            _mockSignEncodingKey = new Mock<IJwtSigningEncodingKey>();
            _mockEncryptEncodingKey = new Mock<IJwtEncryptingEncodingKey>();

            _authModel = new AuthModel() { Email = "test@mail.com", Password = "1234567A" };

            _authService = new AuthService(
                _mockMongoManager.Object,
                _mockLogger.Object,
                _mockHasher.Object,
                _mockSignEncodingKey.Object,
                _mockEncryptEncodingKey.Object
                );
        }

        public static IEnumerable<object[]> UsersData
        {
            get
            {
                IEnumerable<object[]> users = new List<object[]>()
                {
                    new object[] { "other@mail.com" , "qweasdzxc" },
                    new object[] { "test@mail.com" , "123qweasd" }
                };

                return users;
            }
        }

        #region Authorization method

        [Fact]
        public async Task Should_ReturnFalse_When_UserNonexistAsync()
        {
            //Arrange
            _mockMongoManager.Setup(x => x.UserManager.GetAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            //Act
            var result = await _authService.AuthorizeAsync(_authModel);

            //Assert
            Assert.False(result.IsSuccessful);
        }

        [Theory, MemberData(nameof(UsersData))]
        public async Task Should_ReturnFalse_When_DataIncorrectAsync(string email, string hash)
        {
            //Arrange            
            var user = new User() { Email = email, PasswordHash = hash };
            string generatedhash = "qweasdzxc";

            _mockMongoManager.Setup(x => x.UserManager.GetAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockHasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(generatedhash);

            //Act
            var result = await _authService.AuthorizeAsync(_authModel);

            //Assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Should_ReturnTrue_When_DataCorrectAsync()
        {
            //Arrange            
            var user = new User() { Email = "test@mail.com", PasswordHash = "qweasdzxc" };
            string generatedhash = "qweasdzxc";

            _mockMongoManager.Setup(x => x.UserManager.GetAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockHasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(generatedhash);

            //Act
            var result = await _authService.AuthorizeAsync(_authModel);

            //Assert
            Assert.True(result.IsSuccessful);
        }

        #endregion

        #region Check user data method

        [Fact]
        public async Task Should_ReturnTrue_When_UserExistAsync()
        {
            //Arrange
            _mockMongoManager.Setup(x => x.UserManager.CheckAsync(It.Is<string>(y => y != null &&  y != string.Empty))).ReturnsAsync(true);

            //Act
            var result = await _authService.CheckUserAsync("test@mail.com");

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Should_ReturnFalse_When_UserDoesntExistAsync()
        {
            //Arrange
            _mockMongoManager.Setup(x => x.UserManager.CheckAsync(It.Is<string>(y => y != null && y != string.Empty))).ReturnsAsync(false);

            //Act
            var result = await _authService.CheckUserAsync("test@mail.com");

            //Assert
            Assert.False(result);
        }

        #endregion

        #region Generate token method

        [Fact]
        public async Task Should_ReturnNull_When_UserDoentExistAsync()
        {
            //Arrange            
            _mockMongoManager.Setup(x => x.UserManager.GetAsync(It.Is<string>(e => e != null && e != string.Empty))).ReturnsAsync((User)null);

            //Act
            var result = await _authService.GetTokenAsync("test@mail.ru");

            //Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Obj);
        }

        #endregion
    }
}
